// src/app/models/event.model.ts
export interface Event {
  eventId: string;
  eventName: string;
  eventCategory: string;
  eventDate: Date;
  description: string;
  status: string;
  location: string;
  maxParticipants?: number;
  participantCount?: number;
  createdDate?: Date;
  sessionCount?: number;
  lastModifiedDate?: Date;
}

export interface EventAdminDto {
  eventId: string;
  eventName: string;
  eventCategory: string;
  eventDate: string;
  status: string;
  location: string;
  maxParticipants: number;
  participantCount: number;
  sessionCount: number;
  createdDate: string;
  lastModifiedDate: string;
}

export interface CreateEventRequest {
  eventName: string;
  eventCategory: string;
  eventDate: string;
  description: string;
  location: string;
  maxParticipants?: number | null;
}

export interface UpdateEventRequest extends CreateEventRequest {
  eventId: string;
}

export interface PaginatedEventResponse {
  data: Event[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

export interface EventResponse {
  success: boolean;
  message: string;
  data: Event;
  statusCode: number;
}

export interface EventListResponse {
  success: boolean;
  message: string;
  data: PaginatedEventResponse;
  statusCode: number;
}

export interface AttendanceStatsDto {
  eventId: string;
  totalRegistered: number;
  totalAttended: number;
  totalNoShow: number;
  attendancePercentage: number;
  averageRating: number;
}

