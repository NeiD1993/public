import { Component, Input } from '@angular/core';
import { $enum } from 'ts-enum-util';
import { settingsOptionComponentAnimations } from './settings-option.component.animations';
import { SettingsOptionType } from 'src/app/enums/settings-option-type';
import { SettingsService } from 'src/app/services/settings.service';

@Component({
    selector: 'settings-option-root',
    styleUrls: ['../settings-option.component.css'],
    templateUrl: '../settings-option.component.html',
    animations: [settingsOptionComponentAnimations]
})
export class SettingsOptionComponent {

    private _settingsOptionType: SettingsOptionType = SettingsOptionType.SnakeInitialLength;

    private _step: number = 1;

    private _valueDecoration: string;

    constructor(public settingsService: SettingsService) { }

    @Input()
    label: string;

    @Input()
    set settingsOptionTypeToString(settingsOptionTypeToString: string) {
        this._settingsOptionType = $enum(SettingsOptionType).getValueOrDefault(settingsOptionTypeToString, SettingsOptionType.FieldFoodCountPercentage);
    };

    get settingsOptionType(): SettingsOptionType {
        return this._settingsOptionType;
    }

    get step(): number {
        return this._step;
    }

    @Input()
    set step(step: number) {
        if (step > 0)
            this._step = step;
    }

    get value(): number {
        return this.settingsService.getSettingsOption(this._settingsOptionType);
    }

    get valueToString(): string {
        let value: number = this.value;

        return this._valueDecoration ? (value + this._valueDecoration) : value.toString();
    }

    @Input()
    set valueDecoration(valueDecoration: string) {
        this._valueDecoration = valueDecoration;
    }

    private updateSettingsOption(updatedValue: number): void {
        this.settingsService.setSettingsOption(this._settingsOptionType, updatedValue);
    }

    onBottomChangeButtonClick(): void {
        this.updateSettingsOption(this.value - this.step);
    }

    onTopChangeButtonClick(): void {
        this.updateSettingsOption(this.value + this.step);
    }
}