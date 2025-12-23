import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { User } from '../../models/user.model';

@Component({
  selector: 'app-teacher-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="dashboard-container">
      <header class="dashboard-header">
        <h1>Teacher Dashboard</h1>
        <div class="user-info">
          <span *ngIf="currentUser">Welcome, {{currentUser.firstName}} {{currentUser.lastName}}</span>
          <button (click)="logout()" class="logout-btn">Logout</button>
        </div>
      </header>

      <div class="dashboard-content">
        <div class="stats-grid">
          <div class="stat-card">
            <h3>My Classes</h3>
            <div class="stat-number">8</div>
          </div>
          <div class="stat-card">
            <h3>Active Exams</h3>
            <div class="stat-number">12</div>
          </div>
          <div class="stat-card">
            <h3>Total Students</h3>
            <div class="stat-number">234</div>
          </div>
          <div class="stat-card">
            <h3>Pending Reviews</h3>
            <div class="stat-number">45</div>
          </div>
        </div>

        <div class="teacher-actions">
          <h2>Quick Actions</h2>
          <div class="action-buttons">
            <button class="action-btn">Create New Exam</button>
            <button class="action-btn">View My Classes</button>
            <button class="action-btn">Grade Submissions</button>
            <button class="action-btn">Student Progress</button>
          </div>
        </div>

        <div class="recent-exams">
          <h2>My Recent Exams</h2>
          <div class="exam-list">
            <div class="exam-card">
              <div class="exam-header">
                <h3>Mathematics Midterm</h3>
                <span class="exam-status active">Active</span>
              </div>
              <p class="exam-details">Due: Dec 25, 2024 | Submissions: 23/30</p>
              <div class="exam-actions">
                <button class="btn-sm">View Results</button>
                <button class="btn-sm">Edit</button>
              </div>
            </div>
            <div class="exam-card">
              <div class="exam-header">
                <h3>Physics Quiz #3</h3>
                <span class="exam-status completed">Completed</span>
              </div>
              <p class="exam-details">Completed: Dec 20, 2024 | Submissions: 28/30</p>
              <div class="exam-actions">
                <button class="btn-sm">View Results</button>
                <button class="btn-sm">Analytics</button>
              </div>
            </div>
            <div class="exam-card">
              <div class="exam-header">
                <h3>Chemistry Final</h3>
                <span class="exam-status draft">Draft</span>
              </div>
              <p class="exam-details">Created: Dec 18, 2024 | Questions: 45</p>
              <div class="exam-actions">
                <button class="btn-sm">Continue Editing</button>
                <button class="btn-sm">Preview</button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styleUrls: ['./teacher-dashboard.component.scss']
})
export class TeacherDashboardComponent implements OnInit {
  currentUser: User | null = null;

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
    });
  }

  logout(): void {
    this.authService.logout();
  }
}
