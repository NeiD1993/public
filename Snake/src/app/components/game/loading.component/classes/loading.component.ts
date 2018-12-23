import { BaseGameComponent } from "../../base.game.component/base-game.components";
import { BaseGameLogicService } from "src/app/services/game-logic/base-game-logic.service";
import { Component, ChangeDetectorRef, Input } from "@angular/core";
import { MathService } from "src/app/services/math.service";

@Component({
    selector: 'loading-root',
    styleUrls: ['../loading.component.css'],
    templateUrl: '../loading.component.html'
})
export class LoadingComponent extends BaseGameComponent {

    private _position: number = 0;

    constructor(changeDetector: ChangeDetectorRef, gameLogicService: BaseGameLogicService, private _mathService: MathService) {
        super(changeDetector, gameLogicService, true);
    }

    set position(position: number) {
        if ((position >= 0) && (position <= 100))
            this._position = position;
    }

    get position(): number {
        return this._position;
    }

    get progress(): number {
        return this._mathService.getNumberPercentage(100, this._position / this.gameLogicService.field.bricks.sideBricksCount);
    }

    updatePosition(position: number): void {
        this.position = position;
        this.update();
    }

    @Input()
    text: string;
}