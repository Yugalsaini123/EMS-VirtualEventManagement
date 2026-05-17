//src/app/models/speaker.model.ts
export interface Speaker {
  speakerId: string;
  speakerName: string;
  email: string;
  designation: string;
  organization: string;
  bio: string;
  phoneNumber: string;
  linkedInUrl?: string|null;
  isActive: boolean;
  createdDate?: Date;
}

export interface SpeakerWorkloadDto {
  speakerId: string;
  speakerName: string;
  email: string;
  designation: string;
  organization: string;
  sessionCount: number;
}

export interface CreateSpeakerRequest {
  speakerName: string;
  email: string;
  designation: string;
  organization: string;
  bio: string;
  phoneNumber: string;
  linkedInUrl?: string|null;
}

export interface SpeakerResponse {
  success: boolean;
  message: string;
  data: Speaker;
  statusCode: number;
}

export interface SpeakerListResponse {
  success: boolean;
  message: string;
  data: Speaker[];
  statusCode: number;
}
