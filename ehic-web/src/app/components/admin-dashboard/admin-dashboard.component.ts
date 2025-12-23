import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { User } from '../../models/user.model';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="dashboard-container">
      <header class="dashboard-header">
        <h1>Admin Dashboard</h1>
        <div class="user-info">
          <span *ngIf="currentUser">Welcome, {{currentUser.firstName}} {{currentUser.lastName}}</span>
          <button (click)="logout()" class="logout-btn">Logout</button>
        </div>
      </header>

      <div class="dashboard-content">
        <div class="stats-grid">
          <div class="stat-card">
            <h3>Total Users</h3>
            <div class="stat-number">1,234</div>
          </div>
          <div class="stat-card">
            <h3>Active Exams</h3>
            <div class="stat-number">56</div>
          </div>
          <div class="stat-card">
            <h3>Teachers</h3>
            <div class="stat-number">89</div>
          </div>
          <div class="stat-card">
            <h3>Students</h3>
            <div class="stat-number">1,089</div>
          </div>
        </div>

        <div class="admin-actions">
          <h2>Admin Actions</h2>
          <div class="action-buttons">
            <button class="action-btn">Manage Users</button>
            <button class="action-btn">Create Exam</button>
            <button class="action-btn">View Reports</button>
            <button class="action-btn">System Settings</button>
          </div>
        </div>

        <div class="recent-activity">
          <h2>Recent Activity</h2>
          <div class="activity-list">
            <div class="activity-item">
              <span class="activity-time">2 hours ago</span>
              <span class="activity-text">New teacher registered: John Smith</span>
            </div>
            <div class="activity-item">
              <span class="activity-time">4 hours ago</span>
              <span class="activity-text">Exam "Mathematics Final" was completed by 45 students</span>
            </div>
            <div class="activity-item">
              <span class="activity-time">6 hours ago</span>
              <span class="activity-text">System backup completed successfully</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styleUrls: ['./admin-dashboard.component.scss']
})
export class AdminDashboardComponent implements OnInit {
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
