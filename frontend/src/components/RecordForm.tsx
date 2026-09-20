/**
 * RecordForm Component
 * Modal form for creating and editing records
 */

import { useState, useEffect } from 'react';
import type { RecordDto } from '../services/recordService';
import '../styles/RecordForm.css';

interface RecordFormProps {
  record: RecordDto | null;
  onSubmit: (data: { title: string; content: string }) => Promise<void>;
  onClose: () => void;
}

export function RecordForm({ record, onSubmit, onClose }: RecordFormProps) {
  const [formData, setFormData] = useState({
    title: '',
    content: '',
  });
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [validationError, setValidationError] = useState('');

  useEffect(() => {
    if (record) {
      setFormData({
        title: record.title,
        content: record.content,
      });
    } else {
      setFormData({ title: '', content: '' });
    }
    setValidationError('');
  }, [record]);

  const validateForm = (): boolean => {
    if (!formData.title.trim()) {
      setValidationError('Title is required');
      return false;
    }
    if (formData.title.trim().length < 3) {
      setValidationError('Title must be at least 3 characters');
      return false;
    }
    if (!formData.content.trim()) {
      setValidationError('Content is required');
      return false;
    }
    return true;
  };

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
  ) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
    setValidationError('');
  };

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    if (!validateForm()) {
      return;
    }

    setIsSubmitting(true);

    try {
      await onSubmit(formData);
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleBackdropClick = (e: React.MouseEvent<HTMLDivElement>) => {
    if (e.target === e.currentTarget) {
      onClose();
    }
  };

  return (
    <div className="modal-backdrop" onClick={handleBackdropClick}>
      <div className="modal-content">
        <div className="modal-header">
          <h2 className="modal-title">
            {record ? 'Edit Record' : 'Create New Record'}
          </h2>
          <button
            type="button"
            className="modal-close"
            onClick={onClose}
            aria-label="Close form"
          >
            ×
          </button>
        </div>

        <form onSubmit={handleSubmit} className="record-form">
          {validationError && (
            <div className="error-message" role="alert">
              {validationError}
            </div>
          )}

          <div className="form-group">
            <label htmlFor="title" className="form-label">
              Title *
            </label>
            <input
              id="title"
              type="text"
              name="title"
              value={formData.title}
              onChange={handleChange}
              placeholder="Enter record title"
              disabled={isSubmitting}
              className="form-input"
              required
            />
          </div>

          <div className="form-group">
            <label htmlFor="content" className="form-label">
              Content *
            </label>
            <textarea
              id="content"
              name="content"
              value={formData.content}
              onChange={handleChange}
              placeholder="Enter record content"
              disabled={isSubmitting}
              className="form-textarea"
              rows={8}
              required
            />
          </div>

          <div className="form-actions">
            <button
              type="button"
              onClick={onClose}
              disabled={isSubmitting}
              className="btn btn-secondary"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={isSubmitting}
              className="btn btn-primary"
            >
              {isSubmitting
                ? record
                  ? 'Updating...'
                  : 'Creating...'
                : record
                ? 'Update Record'
                : 'Create Record'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
