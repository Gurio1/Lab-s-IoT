export interface HealthStatus {
  status: string;
  totalDuration: string;
  entries: {
    'iot-connections': {
      data: {
        [key: string]: {
          mqttConnection: string;
          networkConnection: string;
        };
      };
      description: string;
      duration: string;
      status: string;
      tags: string[];
    };
  };
}
