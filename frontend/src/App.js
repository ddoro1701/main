import React, { useEffect, useState } from 'react';
import './App.css';
import config from './config';

function App() {
  const [packages, setPackages] = useState([]);

  useEffect(() => {
    fetch(`${config.backendUrl}/package`)
      .then(response => response.json())
      .then(data => setPackages(data))
      .catch(error => console.error('Error fetching packages:', error));
  }, []);

  return (
    <div className="App">
      <header className="App-header">
        <h1>Welcome to the University Package Management System</h1>
        <ul>
          {packages.map(pkg => (
            <li key={pkg.id}>{pkg.name}</li>
          ))}
        </ul>
      </header>
    </div>
  );
}

export default App;
