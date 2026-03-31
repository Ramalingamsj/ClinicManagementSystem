import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {
  stats = [
    { label: 'PATIENTS SERVED', value: '12,480+' },
    { label: 'CLINICAL ACCURACY', value: '99.9%' },
    { label: 'NEURAL UPTIME', value: '24/7' }
  ];

  features = [
    {
      title: 'Doctor Console',
      desc: 'Precision diagnostic workstation with neural-link consultation tracking.',
      icon: 'fa-user-md',
      color: 'blue'
    },
    {
      title: 'Pharmacy Hub',
      desc: 'Automated medicine dispensing and real-time inventory synchronization.',
      icon: 'fa-pills',
      color: 'purple'
    },
    {
      title: 'Lab Analytics',
      desc: 'High-fidelity test automation with instant result waveform generation.',
      icon: 'fa-microscope',
      color: 'emerald'
    },
    {
      title: 'Patient Registry',
      desc: 'Seamless receptionist intake with intelligent appointment queuing.',
      icon: 'fa-id-card',
      color: 'cyan'
    }
  ];
}
