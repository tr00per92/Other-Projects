import {Component, OnInit} from 'angular2/core';
import {Hero} from '../common/hero';
import {HeroService} from '../services/hero.service';
import {RouteParams} from 'angular2/router';

@Component({
  selector: 'my-hero-detail',
  templateUrl : 'templates/hero-detail.component.html',
  inputs: ['hero'],
  styles: ['styles/hero-detail.component.css']
})

export class HeroDetailComponent implements OnInit {
    constructor(private heroService: HeroService, private routeParams: RouteParams) { }
    hero : Hero;
    ngOnInit() {
        let id = +this.routeParams.get('id');
        this.heroService.getHero(id).then(hero => this.hero = hero);
    }
    goBack() {
        window.history.back();
    }
}
