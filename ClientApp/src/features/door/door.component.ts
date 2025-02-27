import { Component, AfterViewInit, HostListener } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-door',
  imports: [FormsModule],
  templateUrl: './door.component.html',
  styleUrl: './door.component.scss',
})
export class DoorComponent implements AfterViewInit {
  isChecked = false;
  private particles: any[] = [];
  private canvas!: HTMLCanvasElement;
  private ctx!: CanvasRenderingContext2D;

  ngAfterViewInit(): void {
    this.canvas = document.getElementById(
      'particle-canvas'
    ) as HTMLCanvasElement;
    this.ctx = this.canvas.getContext('2d')!;
    this.createParticles();
    this.animateParticles();
  }

  @HostListener('window:resize')
  onResize() {
    this.createParticles(); // Re-draw particles on resize
  }

  createParticles() {
    this.canvas.width = window.innerWidth;
    this.canvas.height = window.innerHeight;

    this.particles = [];
    const particleCount = Math.floor(
      (window.innerWidth * window.innerHeight) / 15000
    ); // Dynamic density

    for (let i = 0; i < particleCount; i++) {
      const x = Math.random() * this.canvas.width;
      const y = Math.random() * this.canvas.height;
      const radius = Math.random() * 3;
      const speed = Math.random() * 0.5;
      this.particles.push({ x, y, radius, speed });
    }
  }

  animateParticles() {
    this.ctx.clearRect(0, 0, this.canvas.width, this.canvas.height);

    for (let particle of this.particles) {
      particle.y -= particle.speed;
      if (particle.y < 0) particle.y = this.canvas.height;

      this.ctx.beginPath();
      this.ctx.arc(particle.x, particle.y, particle.radius, 0, 2 * Math.PI);
      this.ctx.fillStyle = `rgba(255, 255, 255, 0.7)`;
      this.ctx.shadowBlur = 5;
      this.ctx.shadowColor = 'white';
      this.ctx.fill();
    }

    requestAnimationFrame(() => this.animateParticles());
  }

  onCheckboxChange(): void {
    if (this.isChecked) {
      setTimeout(() => {
        this.isChecked = false; // Uncheck after 3 seconds
      }, 3000);
    }
  }
}
