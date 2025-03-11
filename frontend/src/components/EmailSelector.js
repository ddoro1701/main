import React, { useState, useEffect } from 'react';

const EmailSelector = () => {
  const [emails, setEmails] = useState([]);
  const [newEmail, setNewEmail] = useState('');
  const [selectedEmail, setSelectedEmail] = useState('');

  // Beim Mounten werden die bestehenden E-Mails per GET abgefragt
  useEffect(() => {
    fetch('https://wrexhamuni-ocr-webapp-deeaeydrf2fdcfdy.uksouth-01.azurewebsites.net/api/lecturer/emails')
      .then(res => {
        if (!res.ok) {
          throw new Error("Fehler beim Abrufen der E-Mails");
        }
        return res.json();
      })
      .then(data => {
        // Erwartung: data ist ein Array von Strings, z.B. ["Daniel.Doroschenko@outlook.com", ...]
        setEmails(data);
        if (data.length > 0) setSelectedEmail(data[0]);
      })
      .catch(err => console.error('Error fetching emails:', err));
  }, []);

  // Beim Hinzufügen einer neuen E-Mail wird ein POST-Aufruf ausgeführt
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
        // Es wird angenommen, dass das zurückgegebene Objekt eine Eigenschaft "Email" enthält
        setEmails(prev => [...prev, addedLecturer.Email]);
        setSelectedEmail(addedLecturer.Email);
        setNewEmail('');
      })
      .catch(err => console.error('Error adding email:', err));
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
    </div>
  );
};

export default EmailSelector;