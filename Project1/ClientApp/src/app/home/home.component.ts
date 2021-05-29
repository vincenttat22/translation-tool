import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../services/api.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ["./home.component.css"]
})

export class HomeComponent implements OnInit{
  constructor(private apiService: ApiService) {

  }
  ngOnInit(): void {
    
  }
}
