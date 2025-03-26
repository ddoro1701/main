import React, { useState, useEffect, useCallback } from 'react';
import EmailSelector from './components/EmailSelector';
import PackageLog from './components/PackageLog';

function App() {
    const [text, setText] = useState('');

    const handleImageUpload = async (event) => {
        const file = event.target.files[0];
        if (!file) return;

        const formData = new FormData();
        formData.append('image', file);

        try {
            const response = await fetch('https://wrexhamuni-ocr-webapp-deeaeydrf2fdcfdy.uksouth-01.azurewebsites.net/api/image/upload', {
                method: 'POST',
                body: formData
            });

            if (!response.ok) {
                throw new Error('Fehler beim Upload');
            }

            const result = await response.json();
            setText(result.text);
        } catch (error) {
            console.error('Fehler:', error);
            setText('Error while uploading File');
        }
    };

const [packages, setPackages] = useState([]);

// Function to fetch the updated package list
    // Verwende useCallback, um fetchPackages stabil zu halten
    const fetchPackages = useCallback(() => {
        fetch('https://wrexhamuni-ocr-webapp-deeaeydrf2fdcfdy.uksouth-01.azurewebsites.net/api/package/all')
            .then(res => res.json())
            .then(data => {
                setPackages(data);
            })
            .catch(err => console.error('Error fetching packages:', err));
    }, []); // Keine Abhängigkeiten, da die Funktion stabil bleiben soll

    // useEffect wird nur einmal beim Mount ausgeführt
    useEffect(() => {
        fetchPackages();
    }, [fetchPackages]);

return (
    <div className="App">
        <header className="App-header">
            <h1>Wrexham University Package Management System</h1>
            <input type="file" accept="image/*" onChange={handleImageUpload} />
        </header>
        {/* Pass fetchPackages and packages to the components */}
        <EmailSelector fetchPackages={fetchPackages} />
        <PackageLog packages={packages} fetchPackages={fetchPackages} />
    </div>
);

}
export default App;