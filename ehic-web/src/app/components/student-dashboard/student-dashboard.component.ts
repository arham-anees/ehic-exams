import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { User } from '../../models/user.model';

@Component({
  selector: 'app-student-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="dashboard-container">
      <header class="dashboard-header">
        <h1>Student Dashboard</h1>
        <div class="user-info">
          <span *ngIf="currentUser">Welcome, {{currentUser.firstName}} {{currentUser.lastName}}</span>
          <button (click)="logout()" class="logout-btn">Logout</button>
        </div>
      </header>

      <div class="dashboard-content">
        <div class="stats-grid">
          <div class="stat-card">
            <h3>Enrolled Courses</h3>
            <div class="stat-number">6</div>
          </div>
          <div class="stat-card">
            <h3>Pending Exams</h3>
            <div class="stat-number">3</div>
          </div>
          <div class="stat-card">
            <h3>Completed Exams</h3>
            <div class="stat-number">18</div>
          </div>
          <div class="stat-card">
            <h3>Average Score</h3>
            <div class="stat-number">87%</div>
          </div>
        </div>

        <div class="upcoming-exams">
          <h2>Upcoming Exams</h2>
          <div class="exam-list">
            <div class="exam-card urgent">
              <div class="exam-header">
                <h3>Mathematics Final Exam</h3>
                <span class="exam-date">Due: Dec 25, 2024</span>
              </div>
              <p class="exam-details">Duration: 2 hours | Questions: 50 | Subject: Calculus</p>
              <div class="exam-actions">
                <button class="btn-primary">Take Exam</button>
                <button class="btn-secondary">Study Materials</button>
              </div>
            </div>
            <div class="exam-card">
              <div class="exam-header">
                <h3>Physics Quiz #4</h3>
                <span class="exam-date">Due: Dec 28, 2024</span>
              </div>
              <p class="exam-details">Duration: 45 minutes | Questions: 20 | Subject: Thermodynamics</p>
              <div class="exam-actions">
                <button class="btn-primary">Take Exam</button>
                <button class="btn-secondary">Study Materials</button>
              </div>
            </div>
            <div class="exam-card">
              <div class="exam-header">
                <h3>Chemistry Lab Report</h3>
                <span class="exam-date">Due: Jan 2, 2025</span>
              </div>
              <p class="exam-details">Duration: 1 hour | Questions: 15 | Subject: Organic Chemistry</p>
              <div class="exam-actions">
                <button class="btn-primary">Take Exam</button>
                <button class="btn-secondary">Study Materials</button>
              </div>
            </div>
          </div>
        </div>

        <div class="recent-results">
          <h2>Recent Results</h2>
          <div class="results-table">
            <div class="result-row header">
              <div>Exam</div>
              <div>Date</div>
              <div>Score</div>
              <div>Grade</div>
              <div>Action</div>
            </div>
            <div class="result-row">
              <div>Biology Midterm</div>
              <div>Dec 20, 2024</div>
              <div>92%</div>
              <div class="grade excellent">A</div>
              <div><button class="btn-sm">View Details</button></div>
            </div>
            <div class="result-row">
              <div>History Essay</div>
              <div>Dec 18, 2024</div>
              <div>78%</div>
              <div class="grade good">B+</div>
              <div><button class="btn-sm">View Details</button></div>
            </div>
            <div class="result-row">
              <div>Math Quiz #7</div>
              <div>Dec 15, 2024</div>
              <div>95%</div>
              <div class="grade excellent">A+</div>
              <div><button class="btn-sm">View Details</button></div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styleUrls: ['./student-dashboard.component.scss']
})
export class StudentDashboardComponent implements OnInit {
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
