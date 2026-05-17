import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app.component';
import { appConfig } from './app/app.config';
import 'zone.js';

// Load configuration at runtime
fetch('assets/config.json')
  .then(response => {
    if (!response.ok) {
      throw new Error(`Failed to load config: ${response.status}`);
    }
    return response.json();
  })
  .then(config => {
    // Store config in window object for global access
    (window as any).__CONFIG__ = config;

    // Bootstrap the application
    bootstrapApplication(AppComponent, appConfig)
      .catch((err) => console.error(err));
  })
  .catch(err => {
    console.error('Error loading configuration:', err);
    console.warn('Using default configuration');

    // Provide default config if loading fails
    (window as any).__CONFIG__ = {
      apiUrl: 'http://localhost:5000/api/v1'
    };

    // Bootstrap with defaults
    bootstrapApplication(AppComponent, appConfig)
      .catch((err) => console.error(err));
  });
