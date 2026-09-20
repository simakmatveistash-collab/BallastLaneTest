/**
 * RecordsList Component
 * Displays a grid or list of records with edit and delete actions
 */

import type { RecordDto } from '../services/recordService';
import '../styles/RecordsList.css';

interface RecordsListProps {
  records: RecordDto[];
  onEdit: (record: RecordDto) => void;
  onDelete: (id: number) => void;
}

export function RecordsList({ records, onEdit, onDelete }: RecordsListProps) {
  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString(undefined, {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  return (
    <div className="records-grid">
      {records.map(record => (
        <article key={record.id} className="record-card">
          <div className="record-card-header">
            <h3 className="record-title">{record.title}</h3>
            <div className="record-actions">
              <button
                onClick={() => onEdit(record)}
                className="btn-icon btn-edit"
                title="Edit record"
                aria-label={`Edit record: ${record.title}`}
              >
                ✎
              </button>
              <button
                onClick={() => onDelete(record.id)}
                className="btn-icon btn-delete"
                title="Delete record"
                aria-label={`Delete record: ${record.title}`}
              >
                🗑
              </button>
            </div>
          </div>

          <p className="record-content">{record.content}</p>

          <div className="record-meta">
            <span className="record-date">
              Created: {formatDate(record.createdDate)}
            </span>
            {record.updatedDate !== record.createdDate && (
              <span className="record-date">
                Updated: {formatDate(record.updatedDate)}
              </span>
            )}
          </div>
        </article>
      ))}
    </div>
  );
}
