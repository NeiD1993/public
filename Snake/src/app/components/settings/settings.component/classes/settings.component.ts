import { Component } from '@angular/core';
import { settingsComponentAnimations } from './settings.component.animations';

@Component({
    selector: 'settings-root',
    styleUrls: ['../settings.component.css'],
    templateUrl: '../settings.component.html',    
    animations: [settingsComponentAnimations]
})
export class SettingsComponent { }