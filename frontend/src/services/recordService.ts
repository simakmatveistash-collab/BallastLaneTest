/**
 * Records Service
 * Handles all CRUD operations for records
 */

import { apiClient } from './api';

export interface RecordDto {
  id: number;
  title: string;
  content: string;
  userId: number;
  createdDate: string;
  updatedDate: string;
}

export interface CreateRecordRequest {
  title: string;
  content: string;
}

export interface UpdateRecordRequest {
  title: string;
  content: string;
}

class RecordService {
  async getAllRecords(): Promise<RecordDto[]> {
    const result = await apiClient.get<RecordDto[]>('/api/records');
    return result;
  }

  async getRecordById(id: number): Promise<RecordDto> {
    return apiClient.get<RecordDto>(`/api/records/${id}`);
  }

  async createRecord(data: CreateRecordRequest): Promise<RecordDto> {
    return apiClient.post<RecordDto>('/api/records', data);
  }

  async updateRecord(id: number, data: UpdateRecordRequest): Promise<RecordDto> {
    return apiClient.put<RecordDto>(`/api/records/${id}`, data);
  }

  async deleteRecord(id: number): Promise<void> {
    return apiClient.delete<void>(`/api/records/${id}`);
  }
}

export const recordService = new RecordService();
