import { HubConnectionBuilder } from '@microsoft/signalr';

const connection = new HubConnectionBuilder()
  .withUrl('https://localhost:7223/videocallhub')
  .withAutomaticReconnect()
  .build();

export default connection;
