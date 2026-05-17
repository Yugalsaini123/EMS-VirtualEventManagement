// src/app/app.routes.ts
import { Routes } from '@angular/router';
import { AuthGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: 'login', loadComponent: () => import('./components/login/login.component').then(m => m.LoginComponent) },
  { path: 'register', loadComponent: () => import('./components/register/register.component').then(m => m.RegisterComponent) },
  { path: 'home', loadComponent: () => import('./components/home/home.component').then(m => m.HomeComponent) },
  { path: 'events', loadComponent: () => import('./components/event-list/event-list.component').then(m => m.EventListComponent) },
  { path: 'event-details/:id', loadComponent: () => import('./components/event-details/event-details.component').then(m => m.EventDetailsComponent) },
  { path: 'change-password', canActivate: [AuthGuard], loadComponent: () => import('./components/change-password/change-password.component').then(m => m.ChangePasswordComponent) },
  {
    path: 'admin',
    canActivate: [AuthGuard],
    data: { roles: ['Admin'] },
    children: [
      { path: 'dashboard', loadComponent: () => import('./components/dashboard/dashboard.component').then(m => m.DashboardComponent) },
      { path: 'events', loadComponent: () => import('./components/event-list/event-list.component').then(m => m.EventListComponent) },
      { path: 'event-form', loadComponent: () => import('./components/event-form/event-form.component').then(m => m.EventFormComponent) },
      { path: 'event-form/:id', loadComponent: () => import('./components/event-form/event-form.component').then(m => m.EventFormComponent) },
      { path: 'sessions', loadComponent: () => import('./components/session/session.component').then(m => m.SessionComponent) },
      { path: 'speakers', loadComponent: () => import('./components/speaker/speaker.component').then(m => m.SpeakerComponent) }
    ]
  },
  {
    path: 'participant',
    canActivate: [AuthGuard],
    children: [
      { path: 'events', loadComponent: () => import('./components/participant-events/participant-events.component').then(m => m.ParticipantEventsComponent) }
    ]
  },
  { path: '**', redirectTo: '/home' }
];

