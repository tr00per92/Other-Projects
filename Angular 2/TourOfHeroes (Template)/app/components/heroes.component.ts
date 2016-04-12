import {Component, OnInit} from 'angular2/core';
import {Router} from 'angular2/router';
import {Hero} from '../common/hero';
import {HeroDetailComponent} from './hero-detail.component';
import {HeroService} from '../services/hero.service';

@Component({
    selector: 'my-heroes',
    templateUrl : 'templates/heroes.component.html',
    styleUrls: ['styles/heroes.component.css'],
    directives: [HeroDetailComponent]
})

export class HeroesComponent implements OnInit {
    constructor(private router : Router, private heroService: HeroService) { }
    selectedHero : Hero;
    heroes : Hero[];
    onSelect(hero: Hero) {
        this.selectedHero = hero;
    }
    gotoDetail() {
        this.router.navigate(['HeroDetail', { id: this.selectedHero.id }]);
    }
    ngOnInit() {
        this.heroService.getHeroes().then(heroes => this.heroes = heroes);
    }
}
