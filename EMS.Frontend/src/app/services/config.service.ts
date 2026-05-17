import { Injectable } from '@angular/core';

export interface AppConfig {
  apiUrl: string;
}

@Injectable({
  providedIn: 'root'
})
export class ConfigService {
  private config: AppConfig = {
    apiUrl: 'http://localhost:5000/api/v1'
  };

  constructor() {
    const globalConfig = (window as any).__CONFIG__;
    if (globalConfig && globalConfig.apiUrl) {
      this.config = globalConfig;
    }
  }

  getApiUrl(): string {
    return this.config.apiUrl;
  }

  getConfig(): AppConfig {
    return this.config;
  }
}
