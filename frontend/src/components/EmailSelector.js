import React, { useState, useEffect } from 'react';
import './EmailSelector.css';

const EmailSelector = ({ ocrText, fetchPackages }) => {
  const [emails, setEmails] = useState([]);
  const [fullName, setFullName] = useState('');
  const [newEmail, setNewEmail] = useState('');
  const [selectedEmail, setSelectedEmail] = useState('');
  const [recognizedEmail, setRecognizedEmail] = useState('');
  const [shippingProvider, setShippingProvider] = useState('Amazon');
  const [providerDescription, setProviderDescription] = useState('');
  const [itemCount, setItemCount] = useState(1);
  const [loading, setLoading] = useState(false); // Added loading state
  const [error, setError] = useState(''); // Added error state

  // Load lecturer emails when component mounts.
  useEffect(() => {
    fetch('https://wrexhamuni-ocr-webapp-deeaeydrf2fdcfdy.uksouth-01.azurewebsites.net/api/lecturer/emails')
      .then(res => {
        if (!res.ok) throw new Error("Error fetching emails");
        return res.json();
      })
      .then(data => {
        setEmails(data);
        if (data.length > 0) setSelectedEmail(data[0]);
      })
      .catch(err => console.error('Error fetching emails:', err));
  }, []);

  // Call lecturer matcher when OCR text changes.
  useEffect(() => {
    console.log("[EmailSelector] useEffect triggered with OCR text:", ocrText);
    if (ocrText && ocrText.trim()) {
      setLoading(true); // Start loading
      setError(''); // Clear previous errors
      console.log("Calling /api/label/find-email with OCR text:", ocrText);

      fetch('https://wrexhamuni-ocr-webapp-deeaeydrf2fdcfdy.uksouth-01.azurewebsites.net/api/label/find-email', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(ocrText),
      })
        .then(res => {
          console.log("Response from find-email:", res);
          if (!res.ok) throw new Error("No matching lecturer email found.");
          return res.json();
        })
        .then(data => {
          console.log("Matched OCR email:", data.email);
          setRecognizedEmail(data.email);
        })
        .catch(err => {
          console.error('Error finding email:', err);
          setError('No matching lecturer email found.');
        })
        .finally(() => {
          setLoading(false); // Stop loading
        });
    }
  }, [ocrText]);

  const handleAddEmail = () => {
    if (!newEmail.trim() || !fullName.trim()) {
      alert("Please fill out the full name and the email");
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
        if (!res.ok) throw new Error('Error deleting email');
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
    if (!recognizedEmail) {
      alert("No suggested email available to send.");
      return;
    }

    const packageData = {
      LecturerEmail: recognizedEmail, // Use the suggested email explicitly
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
        alert("Package record created successfully. Email sent to the suggested lecturer.");
        fetchPackages(); // Update the package log
        setItemCount(1); // Reset the item count to 1
        setProviderDescription(""); // Clear the additional information
      })
      .catch(err => {
        console.error('Error sending package data:', err);
      });
  };

  const handleSendEmailWithChosenEmail = () => {
    const packageData = {
      LecturerEmail: selectedEmail,
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
        if (!res.ok) throw new Error('Error sending package data with chosen email');
        return res.json();
      })
      .then(data => {
        alert("Package record created successfully. Email sent to chosen lecturer.");
        fetchPackages(); // Update the package log
        setItemCount(1); // Reset the item count to 1
        setProviderDescription(""); // Clear the additional information
      })
      .catch(err => {
        console.error('Error sending package data with chosen email:', err);
      });
  };

  return (
    <div className="email-selector">
      {/* Send Email/Log Information section above email selection */}
      <div className="send-email-section">
        {loading && <p>Processing... Please wait.</p>} {/* Show loading indicator */}
        {error && <p style={{ color: 'red' }}>{error}</p>} {/* Show error message */}
        {!loading && recognizedEmail && (
          <p>Suggested Lecturer Email: {recognizedEmail}</p>
        )}
        <button onClick={handleSendEmail}>Send Email/Log Information</button>
      </div>

      {/* Email selection and New Lecturer section */}
      <div className="email-container">
        <div className="email-select">
          <h2>Choose an Email</h2>
          <select value={selectedEmail} onChange={e => setSelectedEmail(e.target.value)}>
            {emails.map((email, index) => (
              <option key={index} value={email}>{email}</option>
            ))}
          </select>
          <button onClick={handleDeleteEmail}>Delete chosen E-Mail</button>
        </div>

        <div className="new-lecturer">
          <h2>Add new Lecturer</h2>
          <input
            type="text"
            placeholder="Full Name"
            value={fullName}
            onChange={e => setFullName(e.target.value)}
          />
          <input
            type="email"
            placeholder="New E-Mail"
            value={newEmail}
            onChange={e => setNewEmail(e.target.value)}
          />
          <button onClick={handleAddEmail}>Add Lecturer</button>
        </div>
      </div>

      {/* Shipping Provider and Item Count section */}
      <div className="shipping-container">
        <div className="item-count">
          <h4>Choose item count</h4>
          <select value={itemCount} onChange={e => setItemCount(e.target.value)}>
            {[...Array(10)].map((_, i) => (
              <option key={i + 1} value={i + 1}>{i + 1}</option>
            ))}
          </select>
        </div>
        <div className="shipping-provider">
          <h4>Shipping Provider</h4>
          <select value={shippingProvider} onChange={e => setShippingProvider(e.target.value)}>
            <option value="Royal Mail">Royal Mail</option>
            <option value="Amazon">Amazon</option>
            <option value="DPD">DPD</option>
            <option value="FedEx">FedEx</option>
            <option value="UPS">UPS</option>
            <option value="Evri">Evri</option>
            <option value="Other">Other</option>
          </select>
        </div>
      </div>

      {/* Additional Information Box and new send button under it */}
      <div className="additional-info-section" style={{ marginTop: '20px', textAlign: 'center' }}>
        <h4>Additional Information</h4>
        <textarea
          placeholder="Type your custom information here..."
          value={providerDescription}
          onChange={e => setProviderDescription(e.target.value)}
          style={{ width: '90%', height: '60px', padding: '0.5em', borderRadius: '4px', border: '1px solid #ccc' }}
        ></textarea>
        <button onClick={handleSendEmailWithChosenEmail} style={{ marginTop: '10px' }}>
          Send Email/Log Information (Chosen Email)
        </button>
      </div>
    </div>
  );
};

export default EmailSelector;