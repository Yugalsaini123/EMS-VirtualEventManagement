//src/app/app-routing.module.ts
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
 
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { HomeComponent } from './components/home/home.component';
import { EventListComponent } from './components/event-list/event-list.component';
import { EventFormComponent } from './components/event-form/event-form.component';
import { EventDetailsComponent } from './components/event-details/event-details.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { SessionComponent } from './components/session/session.component';
import { SpeakerComponent } from './components/speaker/speaker.component';
import { ParticipantEventsComponent } from './components/participant-events/participant-events.component';
 
import { AuthGuard } from './guards/auth.guard';
 
const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'home', component: HomeComponent },
  { path: 'events', component: EventListComponent },
  { path: 'event-details/:id', component: EventDetailsComponent },
  {
    path: 'admin',
    canActivate: [AuthGuard],
    data: { roles: ['Admin'] },
    children: [
      { path: 'dashboard', component: DashboardComponent },
      { path: 'events', component: EventListComponent },
      { path: 'event-form', component: EventFormComponent },
      { path: 'event-form/:id', component: EventFormComponent },
      { path: 'sessions', component: SessionComponent },
      { path: 'speakers', component: SpeakerComponent }
    ]
  },
  {
    path: 'participant',
    canActivate: [AuthGuard],
    children: [
      { path: 'events', component: ParticipantEventsComponent }
    ]
  },
  { path: '**', redirectTo: '/home' }
];
 
@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
