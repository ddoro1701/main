import React, { useEffect } from 'react';
import LogTable from './LogTable';

const PackageLog = ({ packages, fetchPackages }) => {
    const handleToggleStatus = (packageItem) => {
        const updatedStatus = packageItem.status === 'Received' ? 'Collected' : 'Received';
        fetch('https://wrexhamuni-ocr-webapp-deeaeydrf2fdcfdy.uksouth-01.azurewebsites.net/api/package/update-status', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ id: packageItem.id, status: updatedStatus }),
        })
            .then(res => res.json())
            .then(() => {
                fetchPackages(); // Refresh the package list after status update
            })
            .catch(err => console.error('Error updating status:', err));
    };

    const handleDeleteCollected = (deletedIds) => {
        fetchPackages(); // Refresh the package list after deletion
    };

    useEffect(() => {
        fetchPackages();
    }, [fetchPackages]);

    return (
        <div>
            <h1>Package Log</h1>
            <LogTable
                data={packages}
                onToggleStatus={handleToggleStatus}
                onDeleteCollected={handleDeleteCollected}
            />
        </div>
    );
};

export default PackageLog;