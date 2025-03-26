import React, { useState, useEffect } from 'react';
import LogTable from './LogTable';

const PackageLog = () => {
    const [packages, setPackages] = useState([]);

    useEffect(() => {
        fetch('https://wrexhamuni-ocr-webapp-deeaeydrf2fdcfdy.uksouth-01.azurewebsites.net/api/package/all')
            .then(res => res.json())
            .then(data => {
                setPackages(data);
            })
            .catch(err => console.error('Error fetching packages:', err));
    }, []);

    const handleToggleStatus = (packageItem) => {
        const updatedStatus = packageItem.status === 'Received' ? 'Collected' : 'Received';
        fetch('https://wrexhamuni-ocr-webapp-deeaeydrf2fdcfdy.uksouth-01.azurewebsites.net/api/package/update-status', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ id: packageItem.id, status: updatedStatus }),
        })
            .then(res => res.json())
            .then(() => {
                setPackages(prev =>
                    prev.map(pkg =>
                        pkg.id === packageItem.id ? { ...pkg, status: updatedStatus } : pkg
                    )
                );
            })
            .catch(err => console.error('Error updating status:', err));
    };

    const handleDeleteCollected = (deletedIds) => {
        // remove entries from state
        setPackages(prev => prev.filter(pkg => !deletedIds.includes(pkg.id)));
    };

    return (
        <div>
            <h1>Package Log</h1>
            <LogTable data={packages} onToggleStatus={handleToggleStatus} onDeleteCollected={handleDeleteCollected} />
        </div>
    );
};

export default PackageLog;