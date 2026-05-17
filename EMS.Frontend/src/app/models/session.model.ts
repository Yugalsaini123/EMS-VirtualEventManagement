//src/app/models/session.model.ts
export interface Session {
  sessionId: string;
  eventId: string;
  sessionTitle: string;
  description?: string;
  sessionStart: Date;
  sessionEnd: Date;
  location: string;
  status: string;
  speakerId?: string;
  speakerName: string;
  createdDate?: Date;
  sessionUrl?: string;
}

export interface CreateSessionRequest {
  eventId: string;
  title: string;         
  description: string;
  startTime: Date;       
  endTime: Date;          
  location: string;
  speakerId?: string;
  sessionUrl?: string;     
}

export interface SessionResponse {
  success: boolean;
  message: string;
  data: Session;
  statusCode: number;
}

export interface SessionListResponse {
  success: boolean;
  message: string;
  data: Session[];
  statusCode: number;
}

