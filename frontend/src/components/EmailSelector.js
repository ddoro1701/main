import React, { useState, useEffect } from 'react';

const EmailSelector = () => {
  const [emails, setEmails] = useState([]);
  const [newEmail, setNewEmail] = useState('');
  const [selectedEmail, setSelectedEmail] = useState('');
  const [shippingProvider, setShippingProvider] = useState('Amazon');
  const [providerDescription, setProviderDescription] = useState(''); // Empty by default

  useEffect(() => {
    fetch('https://wrexhamuni-ocr-webapp-deeaeydrf2fdcfdy.uksouth-01.azurewebsites.net/api/lecturer/emails')
      .then(res => {
        if (!res.ok) {
          throw new Error("Fehler beim Abrufen der E-Mails");
        }
        return res.json();
      })
      .then(data => {
        setEmails(data);
        if (data.length > 0) setSelectedEmail(data[0]);
      })
      .catch(err => console.error('Error fetching emails:', err));
  }, []);

  // POST a new email
  const handleAddEmail = () => {
    if (!newEmail.trim()) return;
    const lecturerData = { Email: newEmail };

    fetch('https://wrexhamuni-ocr-webapp-deeaeydrf2fdcfdy.uksouth-01.azurewebsites.net/api/lecturer/emails', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(lecturerData)
    })
      .then(res => {
        if (!res.ok) {
          throw new Error('Fehler beim Hinzufügen der E-Mail');
        }
        return res.json();
      })
      .then(addedLecturer => {
        setEmails(prev => [...prev, addedLecturer.Email]);
        setSelectedEmail(addedLecturer.Email);
        setNewEmail('');
      })
      .catch(err => console.error('Error adding email:', err));
  };

  // DELETE the selected email
  const handleDeleteEmail = () => {
    if (!selectedEmail) return;
    fetch(`https://wrexhamuni-ocr-webapp-deeaeydrf2fdcfdy.uksouth-01.azurewebsites.net/api/lecturer/emails?email=${encodeURIComponent(selectedEmail)}`, {
      method: 'DELETE'
    })
      .then(res => {
        if (!res.ok) {
          throw new Error('Fehler beim Löschen der E-Mail');
        }
        return res.json();
      })
      .then(() => {
        setEmails(prev => prev.filter(email => email !== selectedEmail));
        const remaining = emails.filter(email => email !== selectedEmail);
        setSelectedEmail(remaining.length > 0 ? remaining[0] : '');
      })
      .catch(err => console.error('Error deleting email:', err));
  };

  const handleUseEmail = () => {
    // Customize the behavior for the "Use Email" button as needed.
    alert(`Using email: ${selectedEmail}`);
  };

  return (
    <div>
      <h2>Choose an E-Mail</h2>
      <select
        value={selectedEmail}
        onChange={e => setSelectedEmail(e.target.value)}
      >
        {emails.map((email, index) => (
          <option key={index} value={email}>{email}</option>
        ))}
      </select>
      <h3>Add new E-Mail</h3>
      <input
        type="email"
        placeholder="New E-Mail"
        value={newEmail}
        onChange={e => setNewEmail(e.target.value)}
      />
      <button onClick={handleAddEmail}>Add E-Mail</button>
      <br />
      <button onClick={handleDeleteEmail}>Delete chosen E-Mail</button>

      {/* Shipping Provider Section */}
      <div style={{ marginTop: '1em' }}>
        <h3>Choose a Shipping Provider</h3>
        <select
          value={shippingProvider}
          onChange={e => setShippingProvider(e.target.value)}
        >
          <option value="Royal Mail">Royal Mail</option>
          <option value="Amazon">Amazon</option>
          <option value="DPD">DPD</option>
          <option value="FedEx">FedEx</option>
          <option value="UPS">UPS</option>
          <option value="Evri">Evri</option>
          <option value="Other">Other</option>
        </select>

        {/* Empty Description Box */}
        <div style={{ marginTop: '1em' }}>
          <h4>Enter additional information</h4>
          <textarea
            placeholder="Type your custom information here..."
            value={providerDescription}
            onChange={e => setProviderDescription(e.target.value)}
            style={{ width: '100%', height: '80px', padding: '0.5em' }}
          ></textarea>
        </div>
      </div>

      {/* Additional block: If an email is found, display it and show "Use Email" button */}
      {selectedEmail && (
        <div style={{ marginTop: '1em' }}>
          <p>Found Email: {selectedEmail}</p>
          <button onClick={handleUseEmail}>Use Email</button>
        </div>
      )}
    </div>
  );
};

export default EmailSelector;