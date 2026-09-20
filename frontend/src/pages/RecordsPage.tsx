/**
 * RecordsPage Component
 * Main page for displaying and managing user records with CRUD operations
 */

import { useState, useEffect } from 'react';
import { recordService } from '../services/recordService';
import type { RecordDto } from '../services/recordService';
import { RecordForm } from '../components/RecordForm';
import { RecordsList } from '../components/RecordsList';
import '../styles/Records.css';

export function RecordsPage() {
  const [records, setRecords] = useState<RecordDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [isFormOpen, setIsFormOpen] = useState(false);
  const [editingRecord, setEditingRecord] = useState<RecordDto | null>(null);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);

  const loadRecords = async () => {
    setIsLoading(true);
    setError(null);

    try {
      const data = await recordService.getAllRecords();
      setRecords(data);
    } catch (err) {
      const message = err instanceof Error ? err.message : 'Failed to load records';
      setError(message);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    console.log('RecordsPage mounted, loading records...');
    loadRecords();
  }, []);

  const handleCreateClick = () => {
    setEditingRecord(null);
    setIsFormOpen(true);
  };

  const handleEditClick = (record: RecordDto) => {
    setEditingRecord(record);
    setIsFormOpen(true);
  };

  const handleFormClose = () => {
    setIsFormOpen(false);
    setEditingRecord(null);
  };

  const handleFormSubmit = async (data: { title: string; content: string }) => {
    try {
      setError(null);

      if (editingRecord) {
        const updated = await recordService.updateRecord(editingRecord.id, data);
        setRecords(records.map(r => r.id === updated.id ? updated : r));
        setSuccessMessage('Record updated successfully');
      } else {
        const created = await recordService.createRecord(data);
        setRecords([created, ...records]);
        setSuccessMessage('Record created successfully');
      }

      handleFormClose();

      // Clear success message after 3 seconds
      setTimeout(() => setSuccessMessage(null), 3000);
    } catch (err) {
      const message = err instanceof Error ? err.message : 'Failed to save record';
      setError(message);
    }
  };

  const handleDeleteRecord = async (id: number) => {
    if (!confirm('Are you sure you want to delete this record?')) {
      return;
    }

    try {
      await recordService.deleteRecord(id);
      setRecords(records.filter(r => r.id !== id));
      setSuccessMessage('Record deleted successfully');

      // Clear success message after 3 seconds
      setTimeout(() => setSuccessMessage(null), 3000);
    } catch (err) {
      const message = err instanceof Error ? err.message : 'Failed to delete record';
      setError(message);
    }
  };

  return (
    <div className="records-container">
      <div className="records-header">
        <h1 className="records-title">My Records</h1>
        <button
          onClick={handleCreateClick}
          className="btn btn-primary"
        >
          + New Record
        </button>
      </div>

      {error && (
        <div className="error-message" role="alert">
          {error}
          <button
            className="close-button"
            onClick={() => setError(null)}
            aria-label="Close error message"
          >
            ×
          </button>
        </div>
      )}

      {successMessage && (
        <div className="success-message" role="status">
          {successMessage}
          <button
            className="close-button"
            onClick={() => setSuccessMessage(null)}
            aria-label="Close success message"
          >
            ×
          </button>
        </div>
      )}

      {isFormOpen && (
        <RecordForm
          record={editingRecord}
          onSubmit={handleFormSubmit}
          onClose={handleFormClose}
        />
      )}

      {isLoading ? (
        <div className="loading" role="status" aria-live="polite">
          <p>Loading records...</p>
        </div>
      ) : records.length === 0 ? (
        <div className="empty-state">
          <h2>No records yet</h2>
          <p>Click "New Record" to create your first record</p>
        </div>
      ) : (
        <RecordsList
          records={records}
          onEdit={handleEditClick}
          onDelete={handleDeleteRecord}
        />
      )}
    </div>
  );
}
