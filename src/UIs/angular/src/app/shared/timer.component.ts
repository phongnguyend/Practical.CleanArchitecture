import { Component, OnInit, OnDestroy } from "@angular/core";

@Component({
  selector: "app-timer",
  templateUrl: "./timer.component.html",
  styleUrls: [],
})
export class TimerComponent implements OnInit, OnDestroy {
  interval: any;
  years: number;
  months: number;
  days: number;
  hours: number;
  minutes: number;
  seconds: number;
  getTime = () => {
    const currentDateTime = new Date();
    this.years = currentDateTime.getFullYear();
    this.months = currentDateTime.getMonth() + 1;
    this.days = currentDateTime.getDate();
    this.hours = currentDateTime.getHours();
    this.minutes = currentDateTime.getMinutes();
    this.seconds = currentDateTime.getSeconds();
  };

  ngOnInit() {
    this.interval = setInterval(() => {
      this.getTime();
    }, 1000);
  }

  ngOnDestroy() {
    clearInterval(this.interval);
  }
}
