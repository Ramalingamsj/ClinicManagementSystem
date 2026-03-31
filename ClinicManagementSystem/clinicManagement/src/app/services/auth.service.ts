import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable, BehaviorSubject, tap } from 'rxjs';
import { LoginResponse } from '../models/auth.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = `${environment.apiUrl}/Login`;
  private currentUserSubject = new BehaviorSubject<LoginResponse | null>(this.getStoredUser());

  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient) {}

  public get currentUserValue(): LoginResponse | null {
    return this.currentUserSubject.value;
  }

  // Uses GET as implemented strictly in the backend C# controller based on user snippet
  login(username: string, password: string): Observable<LoginResponse> {
    return this.http.get<LoginResponse>(`${this.apiUrl}/${username}/${password}`).pipe(
      tap(response => {
        if (response && response.token) {
          localStorage.setItem('currentUser', JSON.stringify(response));
          this.currentUserSubject.next(response);
        }
      })
    );
  }

  logout(): void {
    localStorage.removeItem('currentUser');
    this.currentUserSubject.next(null);
  }

  private getStoredUser(): LoginResponse | null {
    const userJson = localStorage.getItem('currentUser');
    return userJson ? JSON.parse(userJson) : null;
  }

  getToken(): string | null {
    const user = this.getStoredUser();
    return user ? user.token : null;
  }
}
