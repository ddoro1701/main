import React, { useState, useEffect } from 'react';

const EmailSelector = () => {
  const [emails, setEmails] = useState([]);
  const [newEmail, setNewEmail] = useState('');
  const [selectedEmail, setSelectedEmail] = useState('');

  // Load emails on mount
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

  return (
    <div>
      <h2>Wähle eine E-Mail</h2>
      <select
        value={selectedEmail}
        onChange={e => setSelectedEmail(e.target.value)}
      >
        {emails.map((email, index) => (
          <option key={index} value={email}>{email}</option>
        ))}
      </select>
      <h3>Neue E-Mail hinzufügen</h3>
      <input
        type="email"
        placeholder="Neue E-Mail"
        value={newEmail}
        onChange={e => setNewEmail(e.target.value)}
      />
      <button onClick={handleAddEmail}>E-Mail hinzufügen</button>
      <br />
      <button onClick={handleDeleteEmail}>Ausgewählte E-Mail löschen</button>
    </div>
  );
};

export default EmailSelector;