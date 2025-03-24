import React, { useState } from 'react';
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

    return (
        <div className="App">
            <header className="App-header">
                <h1>Package Management System</h1>
                <input type="file" accept="image/*" onChange={handleImageUpload} />
                <p>{text}</p>
            </header>
            {/* Directly include the components */}
            <EmailSelector ocrText={text} />
            <PackageLog />
        </div>
    );
}

export default App;