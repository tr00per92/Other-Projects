import {Component, OnInit} from 'angular2/core';
import {Hero} from '../common/hero';
import {HeroService} from '../services/hero.service';
import {Router} from 'angular2/router';

@Component({
  selector: 'my-dashboard',
  templateUrl : 'templates/dashboard.component.html',
  styleUrls: ['styles/dashboard.component.css']
})

export class DashboardComponent implements OnInit {
    constructor(private router: Router, private heroService: HeroService) { }
    heroes: Hero[];
    ngOnInit() {
        this.heroService.getHeroes().then(heroes => this.heroes = heroes.slice(1, 5));
    }
    gotoDetail(hero: Hero) {
        let link = ['HeroDetail', { id: hero.id }];
        this.router.navigate(link);
    }
 }