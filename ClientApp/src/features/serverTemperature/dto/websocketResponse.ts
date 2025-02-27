export interface ServerTemperatureData {
  topicName: string;
  thermometersData: { [key: string]: string };
  dateTime: Date;
}
