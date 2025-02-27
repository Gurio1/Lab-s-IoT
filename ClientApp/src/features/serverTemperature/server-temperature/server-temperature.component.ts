import { Component, OnInit, OnDestroy, HostListener } from '@angular/core';
import { Chart } from 'chart.js/auto';
import { TempWebsocketService } from '../temp-websocket.service';
import { ServerTemperatureData } from '../dto/websocketResponse';
import { Subscription } from 'rxjs';
import { TemperatureService } from '../temperature.service';
@Component({
  selector: 'app-server-temperature',
  standalone: true,
  imports: [],
  templateUrl: './server-temperature.component.html',
  styleUrl: './server-temperature.component.scss',
})
export class ServerTemperatureComponent implements OnInit, OnDestroy {
  private temperatureSubscription!: Subscription;
  private chart!: Chart;
  private maxEntries = 20;
  private timestamps: string[] = [];
  private temperatureValues: { [key: string]: number[] } = {}; // Store values for each sensor
  private colors = ['#FF5733', '#33C1FF', '#75FF33'];

  constructor(
    private signalRService: TempWebsocketService,
    private temperatureService: TemperatureService
  ) {}

  ngOnInit() {
    this.temperatureService
      .fetchLastTemperatureData(this.maxEntries - 1)
      .subscribe((data) => {
        this.createChart(data);
      });

    this.temperatureSubscription = this.signalRService
      .getTemperatureData()
      .subscribe((data) => {
        this.updateChart(data);
      });
  }

  private createChart(data: ServerTemperatureData[]) {
    const ctx = document.getElementById(
      'temperatureChart'
    ) as HTMLCanvasElement;

    this.chart = new Chart(ctx, {
      type: 'line',
      data: {
        labels: [],
        datasets: [],
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        scales: {
          x: {
            title: { display: true, text: 'Time', color: '#fff' },
            grid: {
              color: '#e0e0e0', // Light grid color for better visual appearance
            },
            ticks: {
              color: '#fff', // Label color
            },
          },
          y: {
            title: { display: true, text: 'Temperature (°C)', color: '#fff' },
            grid: {
              color: '#e0e0e0',
            },
            ticks: {
              color: '#fff',
            },
          },
        },
        elements: {
          line: {
            tension: 0.2, // Set to 0 for a straight line (linear)
            borderCapStyle: 'round', // Rounded line ends
            borderJoinStyle: 'round', // Rounded line joints
          },
        },
        plugins: {
          legend: {
            position: 'top',
            labels: {
              color: '#fff', // Legend text color
              font: {
                weight: 'bold',
              },
            },
          },
          tooltip: {
            backgroundColor: 'rgba(38, 4, 25, 0.7)', // Dark background for tooltip
            titleColor: '#fff', // Tooltip title color
            bodyColor: '#fff', // Tooltip body color
            displayColors: false, // Disable color box next to tooltip
            mode: 'index',
            intersect: false,
          },
        },
      },
    });

    this.initializeChartData(data);
  }

  private initializeChartData(data: ServerTemperatureData[]) {
    data.forEach((d) => {
      const currentTime = new Date(d.dateTime).toLocaleTimeString();
      this.timestamps.push(currentTime);

      Object.keys(d.thermometersData).forEach((sensor, index) => {
        const tempValue = parseFloat(d.thermometersData[sensor]);

        if (!this.temperatureValues[sensor]) {
          this.temperatureValues[sensor] = [];
          this.chart.data.datasets.push({
            label: `${sensor}`,
            data: [],
            borderColor: this.colors[index % this.colors.length],
            backgroundColor: 'transparent',
            borderWidth: 3,
            tension: 0.3,
            pointRadius: 5,
            pointBackgroundColor: this.colors[index % this.colors.length],
            fill: false,
          });
        }

        this.temperatureValues[sensor].push(tempValue);
        const datasetIndex = this.chart.data.datasets.findIndex(
          (d) => d.label === `${sensor}`
        );
        if (datasetIndex !== -1) {
          this.chart.data.datasets[datasetIndex].data =
            this.temperatureValues[sensor];
        }
      });
    });

    this.chart.data.labels = this.timestamps;
    this.chart.update();
  }

  private updateChart(data: ServerTemperatureData) {
    const currentTime = new Date(data.dateTime).toLocaleTimeString();

    // Ensure we only keep the last `maxEntries`
    if (this.timestamps.length >= this.maxEntries) {
      this.timestamps.shift();
    }
    this.timestamps.push(currentTime);

    // Loop through thermometersData and update each dataset
    Object.keys(data.thermometersData).forEach((sensor, index) => {
      const tempValue = parseFloat(data.thermometersData[sensor]);

      // Ensure only last `maxEntries` are stored
      if (this.temperatureValues[sensor].length >= this.maxEntries) {
        this.temperatureValues[sensor].shift();
      }
      this.temperatureValues[sensor].push(tempValue);

      // Update dataset data
      const datasetIndex = this.chart.data.datasets.findIndex(
        (d) => d.label === `${sensor}`
      );
      if (datasetIndex !== -1) {
        this.chart.data.datasets[datasetIndex].data =
          this.temperatureValues[sensor];
      }
    });

    // Update chart labels and refresh
    this.chart.data.labels = this.timestamps;
    this.chart.update();
  }

  ngOnDestroy() {
    if (this.temperatureSubscription) {
      this.temperatureSubscription.unsubscribe();
    }
  }
}
