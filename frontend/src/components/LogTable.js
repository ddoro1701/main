import React, { useMemo } from 'react';
import { useTable, useSortBy, usePagination, useFilters } from 'react-table';
import * as XLSX from 'xlsx';
import { saveAs } from 'file-saver';

function DefaultColumnFilter({ column: { filterValue, setFilter } }) {
  return (
    <input
      value={filterValue || ''}
      onChange={e => setFilter(e.target.value || undefined)}
      placeholder="Search..."
      style={{ width: '100%' }}
    />
  );
}

function LogTable({ data, onToggleStatus, onDeleteCollected }) {
  const columns = useMemo(() => [
    {
      Header: 'Lecturer Email',
      accessor: 'lecturerEmail',
    },
    {
      Header: 'Item Count',
      accessor: 'itemCount',
    },
    {
      Header: 'Shipping Provider',
      accessor: 'shippingProvider',
    },
    {
      Header: 'Additional Info',
      accessor: 'additionalInfo',
    },
    {
      Header: 'Collection Date',
      accessor: 'collectionDate',
      Cell: ({ value }) => {
        if (!value) return 'No Date';
        const dateValue = new Date(value);
        return isNaN(dateValue.getTime())
          ? 'Invalid Date'
          : dateValue.toLocaleDateString();
      },
    },
    {
      Header: 'Status',
      accessor: 'status',
    },
    {
      Header: 'Action',
      Cell: ({ row }) => (
        <button onClick={() => onToggleStatus(row.original)}>
          {row.original.status === 'Received' ? 'Mark as Collected' : 'Mark as Received'}
        </button>
      ),
    },
  ], [onToggleStatus]);

  // Include rows from the table instance which represent the current filtered rows.
  const {
    getTableProps,
    getTableBodyProps,
    headerGroups,
    prepareRow,
    page,       // current page rows
    rows,       // all filtered rows
    canPreviousPage,
    canNextPage,
    pageOptions,
    pageCount,
    gotoPage,
    nextPage,
    previousPage,
    setPageSize,
    state: { pageIndex, pageSize },
  } = useTable(
    {
      columns,
      data,
      defaultColumn: { Filter: DefaultColumnFilter },
      initialState: { pageIndex: 0 },
    },
    useFilters,
    useSortBy,
    usePagination
  );

  const exportToExcel = () => {
    // Use the filtered rows (all pages) rather than "data".
    const exportData = rows.map(row => ({
      "Lecturer Email": row.original.lecturerEmail,
      "Item Count": row.original.itemCount,
      "Shipping Provider": row.original.shippingProvider,
      "Additional Info": row.original.additionalInfo,
      "Collection Date": row.original.collectionDate ? new Date(row.original.collectionDate).toLocaleDateString() : '',
      "Status": row.original.status,
    }));

    const worksheet = XLSX.utils.json_to_sheet(exportData, {
      header: ["Lecturer Email", "Item Count", "Shipping Provider", "Additional Info", "Collection Date", "Status"]
    });
    const workbook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(workbook, worksheet, "Package Log");

    const today = new Date();
    const formattedDate = today.toISOString().split('T')[0];
    const fileName = `Package_Logs_${formattedDate}.xlsx`;

    const excelBuffer = XLSX.write(workbook, { bookType: "xlsx", type: "array" });
    const dataBlob = new Blob([excelBuffer], { type: "application/octet-stream" });
    saveAs(dataBlob, fileName);
  };

  const handleDeleteCollected = () => {
    const collectedEntries = page
      .filter(row => row.original.status === "Collected")
      .map(row => row.original.id);

    if (collectedEntries.length === 0) {
      alert('No collected entries found in current view.');
      return;
    }
    if (!window.confirm(`Are you sure you want to delete ${collectedEntries.length} collected entries?`)) {
      return;
    }
    fetch('https://wrexhamuni-ocr-webapp-deeaeydrf2fdcfdy.uksouth-01.azurewebsites.net/api/package/delete-collected', {
      method: 'DELETE',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ ids: collectedEntries })
    })
      .then(res => {
        if (!res.ok) throw new Error('Failed to delete collected entries');
        return res.json();
      })
      .then(() => {
        if (typeof onDeleteCollected === "function") {
          onDeleteCollected(collectedEntries);
        } else {
          alert('Collected entries deleted.');
        }
      })
      .catch(err => {
        console.error('Error deleting collected entries:', err);
        alert('Error deleting collected entries, check console for details.');
      });
  };

  return (
    <div>
      <table {...getTableProps()} style={{ width: '100%', borderCollapse: 'collapse' }}>
      <thead>
  {headerGroups.map(headerGroup => (
    <tr {...headerGroup.getHeaderGroupProps()} style={{ borderBottom: '2px solid #ccc' }}>
      {headerGroup.headers.map(column => (
        <th
          {...column.getHeaderProps(column.getSortByToggleProps())}
          style={{
            padding: '12px',
            backgroundColor: '#f2f2f2',
            textAlign: 'center',
            fontWeight: 'bold',
            color: '#333',
            borderRight: '1px solid #ddd'
          }}
        >
          <div>
            {column.render('Header') === 'Action' ? '' : column.render('Header')}
            <span style={{ marginLeft: '4px', fontSize: '0.8em' }}>
              {column.isSorted ? (column.isSortedDesc ? ' 🔽' : ' 🔼') : ''}
            </span>
          </div>
          {column.canFilter ? (
            <div style={{ marginTop: '6px' }}>
              {/* Customize the filter input appearance */}
              {column.render('Filter', {
                style: {
                  width: '90%',
                  padding: '6px',
                  borderRadius: '4px',
                  border: '1px solid #ccc'
                }
              })}
            </div>
          ) : null}
        </th>
      ))}
    </tr>
  ))}
</thead>
        <tbody {...getTableBodyProps()}>
          {page.map(row => {
            prepareRow(row);
            return (
              <tr {...row.getRowProps()} style={{ borderBottom: '1px solid #eee' }}>
                {row.cells.map(cell => (
                  <td {...cell.getCellProps()} style={{ padding: '8px' }}>
                    {cell.render('Cell')}
                  </td>
                ))}
              </tr>
            );
          })}
        </tbody>
      </table>
      <div style={{ marginTop: '10px' }}>
        <button onClick={() => gotoPage(0)} disabled={!canPreviousPage}>
          {'<<'}
        </button>{' '}
        <button onClick={() => previousPage()} disabled={!canPreviousPage}>
          Previous
        </button>{' '}
        <button onClick={() => nextPage()} disabled={!canNextPage}>
          Next
        </button>{' '}
        <button onClick={() => gotoPage(pageCount - 1)} disabled={!canNextPage}>
          {'>>'}
        </button>{' '}
        <span>
          Page <strong>{pageIndex + 1} of {pageOptions.length}</strong>{' '}
        </span>
        <select
          value={pageSize}
          onChange={e => {
            setPageSize(Number(e.target.value));
          }}
        >
          {[5, 10, 20, 50, 100, 200].map(size => (
            <option key={size} value={size}>
              Show {size}
            </option>
          ))}
        </select>
        <button onClick={exportToExcel} style={{ marginLeft: '10px' }}>Export to Excel</button>
        <button onClick={handleDeleteCollected} style={{ marginLeft: '10px' }}>Delete Collected</button>
      </div>
    </div>
  );
}

export default LogTable;