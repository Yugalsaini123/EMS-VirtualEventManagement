//src/app/models/participant.model.ts
export interface ParticipantEvent {
  registrationId: string;
  eventId: string;
  eventName: string;
  eventDate: Date;
  participantEmail: string;
  registrationDate: Date;
  isAttended: boolean;
  attendanceDate?: Date;
  rating?: number;
  feedback: string;
  status: string;
}

export interface ParticipantInfo {
  emailId: string;
  userName: string;
  role: string;
  isActive: boolean;
  createdAt: Date;
}

export interface RegisterForEventRequest {
  eventId: string;
}

export interface FeedbackRequest {
  registrationId: string;
  rating: number;
  feedback: string;
}

export interface ParticipantResponse {
  success: boolean;
  message: string;
  data: ParticipantEvent;
  statusCode: number;
}

export interface ParticipantListResponse {
  success: boolean;
  message: string;
  data: ParticipantEvent[];
  statusCode: number;
}
