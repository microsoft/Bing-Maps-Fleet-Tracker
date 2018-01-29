import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs/Subscription';
import { Metric } from '../metric';
import { ReportService } from '../report.service';

@Component({
  selector: 'app-report-chart',
  templateUrl: './report-chart.component.html',
  styleUrls: ['./report-chart.component.css']
})
export class ReportChartComponent implements OnInit, OnDestroy {

  metricSubscription: Subscription;
  barChartOptions: any = {
    options: {
      responsive: true,
      scales: {
        yAxes: [{
          ticks: {
            beginAtZero: true
          }
        }]
      }
    },
    chartType: 'bar',
    chartLegendToggle: true,
    chartTitle: '',
    chartLabels: [],
    chartData: [{ data: [] }],
    chartColors: [{
      backgroundColor: '#56A6B6',
      borderColor: '#56A6B6'
    }]
  };

  constructor(private reportService: ReportService) { }

  ngOnInit() {
    this.metricSubscription = this.reportService.getSelectedMetric()
      .subscribe(next => this.onMetricSelected(next));
  }

  ngOnDestroy() {
    this.metricSubscription.unsubscribe();
  }

  private onMetricSelected(metric: Metric) {
    this.barChartOptions.chartTitle = metric.name;

    // Workaround the fact that you cant reassign the chartLabels var
    this.barChartOptions.chartLabels.splice(0, this.barChartOptions.chartLabels.length);

    const data = [];
    for (const key in metric.values) {
      if (metric.values.hasOwnProperty(key)) {
        this.barChartOptions.chartLabels.push(key);
        data.push(metric.values[key]);
      }
    }

    this.barChartOptions.chartData = [{ data: data, label: metric.name + ' (' + metric.units + ')' }];
  }

  // events
  public chartClicked(e: any): void {
    console.log(e);
  }

  public chartHovered(e: any): void {
    console.log(e);
  }
}
