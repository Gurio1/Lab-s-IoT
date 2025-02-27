import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { catchError, Observable, tap, throwError } from 'rxjs';
import { identityTokenResponse } from './dto/identityTokenResponse';
import { loginUser } from './models/loginUser';
import { API_URL, JWT_TOKEN } from '../../constants';

@Injectable({
  providedIn: 'root',
})
export class AuthenticationService {
  constructor(private http: HttpClient, private router: Router) {}

  isLoggedIn: boolean = false;

  login(user: loginUser): Observable<identityTokenResponse> {
    return this.http.post<identityTokenResponse>(API_URL + `login`, user).pipe(
      tap((response: identityTokenResponse) => {
        localStorage.setItem(JWT_TOKEN, response.token);
      }),
      catchError(this.handleError)
    );
  }

  private handleError(error: HttpErrorResponse) {
    if (error.status === 0) {
      this.isLoggedIn = false;
      // A client-side or network error occurred. Handle it accordingly.
      console.error('An error occurred:', error.error);
    } else {
      // The backend returned an unsuccessful response code.
      // The response body may contain clues as to what went wrong.
      console.error(
        `Backend returned code ${error.status}, body was: `,
        error.error
      );
    }
    // Return an observable with a user-facing error message.
    return throwError(
      () => new Error('Something bad happened; please try again later.')
    );
  }
}
