import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { API_URL, JWT_TOKEN } from '../constants';
import { ServerTemperatureData } from './Models/websocketResponse.model';

@Injectable({
  providedIn: 'root',
})
export class TempWebsocketService {
  private hubConnection: signalR.HubConnection;

  constructor() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(API_URL + 'hub/temperature', {
        withCredentials: true,
        accessTokenFactory: () => {
          return localStorage.getItem(JWT_TOKEN)!;
        },
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets,
      })
      .withAutomaticReconnect()
      .build();

    this.startConnection();
  }

  private startConnection() {
    this.hubConnection
      .start()
      .then(() => console.log('Connected to SignalR hub'))
      .catch((err) =>
        //TODO : Catch 401 error code
        console.error('Error connecting to SignalR hub:', err)
      );
  }

  private registerTemperatureListener() {
    this.hubConnection.on(
      'ReceiveTemperature',
      (data: ServerTemperatureData) => {
        this.temperatureSubject.next(data);
      }
    );
  }
}
