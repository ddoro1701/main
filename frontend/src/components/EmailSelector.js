import React, { useState, useEffect } from 'react';

const EmailSelector = ({ ocrText }) => {
  const [emails, setEmails] = useState([]);
  const [fullName, setFullName] = useState('');
  const [newEmail, setNewEmail] = useState('');
  const [selectedEmail, setSelectedEmail] = useState('');
  const [recognizedEmail, setRecognizedEmail] = useState('');
  const [shippingProvider, setShippingProvider] = useState('Amazon');
  const [providerDescription, setProviderDescription] = useState('');
  const [itemCount, setItemCount] = useState(1);

  // Load lecturer emails when component mounts.
  useEffect(() => {
    fetch('https://wrexhamuni-ocr-webapp-deeaeydrf2fdcfdy.uksouth-01.azurewebsites.net/api/lecturer/emails')
      .then(res => {
        if (!res.ok) throw new Error("Fehler beim Abrufen der E-Mails");
        return res.json();
      })
      .then(data => {
        setEmails(data);
        if (data.length > 0) setSelectedEmail(data[0]);
      })
      .catch(err => console.error('Error fetching emails:', err));
  }, []);

  // Immediately call the lecturer matcher when ocrText changes.
  useEffect(() => {
    console.log("New OCR text received:", ocrText);
    if (ocrText && ocrText.trim()) {
      fetch('https://wrexhamuni-ocr-webapp-deeaeydrf2fdcfdy.uksouth-01.azurewebsites.net/api/label/find-email', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(ocrText)
      })
        .then(res => {
          if (!res.ok) throw new Error("Kein passendes Lecturer-Email gefunden.");
          return res.json();
        })
        .then(data => {
          console.log("Matched OCR email:", data.email);
          setRecognizedEmail(data.email);
        })
        .catch(err => {
          console.error('Error finding email:', err);
          // Optionally, clear any old suggestion:
          setRecognizedEmail('');
        });
    }
  }, [ocrText]);

  const handleAddEmail = () => {
    if (!newEmail.trim() || !fullName.trim()) {
      alert("Bitte füllen Sie sowohl den vollen Namen als auch die E-Mail aus.");
      return;
    }
    const lecturerData = { Name: fullName, Email: newEmail };
    fetch('https://wrexhamuni-ocr-webapp-deeaeydrf2fdcfdy.uksouth-01.azurewebsites.net/api/lecturer/emails', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(lecturerData)
    })
      .then(res => {
        if (!res.ok) throw new Error('Fehler beim Hinzufügen der E-Mail');
        return res.json();
      })
      .then(addedLecturer => {
        setEmails(prev => [...prev, addedLecturer.Email]);
        setSelectedEmail(addedLecturer.Email);
        setFullName('');
        setNewEmail('');
      })
      .catch(err => console.error('Error adding email:', err));
  };

  const handleDeleteEmail = () => {
    if (!selectedEmail) return;
    fetch(`https://wrexhamuni-ocr-webapp-deeaeydrf2fdcfdy.uksouth-01.azurewebsites.net/api/lecturer/emails?email=${encodeURIComponent(selectedEmail)}`, {
      method: 'DELETE'
    })
      .then(res => {
        if (!res.ok) throw new Error('Fehler beim Löschen der E-Mail');
        return res.json();
      })
      .then(() => {
        setEmails(prev => prev.filter(email => email !== selectedEmail));
        const remaining = emails.filter(email => email !== selectedEmail);
        setSelectedEmail(remaining.length > 0 ? remaining[0] : '');
      })
      .catch(err => console.error('Error deleting email:', err));
  };

  const handleSendEmail = () => {
    const packageData = {
      LecturerEmail: recognizedEmail || selectedEmail, // Partition Key
      ItemCount: parseInt(itemCount, 10),
      ShippingProvider: shippingProvider,
      AdditionalInfo: providerDescription,
      CollectionDate: new Date(),
    };

    fetch('https://wrexhamuni-ocr-webapp-deeaeydrf2fdcfdy.uksouth-01.azurewebsites.net/api/package/send-email', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(packageData),
    })
      .then(res => {
        if (!res.ok) throw new Error('Error sending package data');
        return res.json();
      })
      .then(data => {
        alert("Package record created successfully.");
      })
      .catch(err => {
        console.error('Error sending package data:', err);
      });
  };

  return (
    <div>
      <h2>Choose an E-Mail</h2>
      <select value={selectedEmail} onChange={e => setSelectedEmail(e.target.value)}>
        {emails.map((email, index) => (
          <option key={index} value={email}>{email}</option>
        ))}
      </select>
      <h3>Add new Lecturer</h3>
      <input
        type="text"
        placeholder="Full Name"
        value={fullName}
        onChange={e => setFullName(e.target.value)}
      />
      <br />
      <input
        type="email"
        placeholder="New E-Mail"
        value={newEmail}
        onChange={e => setNewEmail(e.target.value)}
      />
      <button onClick={handleAddEmail}>Add Lecturer</button>
      <br />
      <button onClick={handleDeleteEmail}>Delete chosen E-Mail</button>

      {/* Always show the "Send Email/Log Information" section */}
      <div style={{ marginTop: '1em', border: '1px solid #ccc', padding: '1em' }}>
        <p>Suggested Lecturer Email: {recognizedEmail || selectedEmail}</p>
        <p>Click below to send email and log information.</p>
        <button onClick={handleSendEmail}>Send Email/Log Information</button>
      </div>

      <div style={{ marginTop: '1em' }}>
        <h3>Choose a Shipping Provider</h3>
        <div style={{ marginTop: '1em' }}>
          <h4>Choose item count</h4>
          <select value={itemCount} onChange={e => setItemCount(e.target.value)}>
            {[...Array(10)].map((_, i) => (
              <option key={i + 1} value={i + 1}>{i + 1}</option>
            ))}
          </select>
        </div>
        <select value={shippingProvider} onChange={e => setShippingProvider(e.target.value)}>
          <option value="Royal Mail">Royal Mail</option>
          <option value="Amazon">Amazon</option>
          <option value="DPD">DPD</option>
          <option value="FedEx">FedEx</option>
          <option value="UPS">UPS</option>
          <option value="Evri">Evri</option>
          <option value="Other">Other</option>
        </select>
        <div style={{ marginTop: '1em' }}>
          <h4>Enter additional information</h4>
          <textarea
            placeholder="Type your custom information here..."
            value={providerDescription}
            onChange={e => setProviderDescription(e.target.value)}
            style={{ width: '50%', height: '80px', padding: '0.5em' }}
          ></textarea>
        </div>
      </div>
    </div>
    
  );
};
export default EmailSelector;