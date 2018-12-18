import { ActivatableGameComponentViewState } from "../../../base.game.component/base-activatable-game.component/base-activatable-game.component.view-state";
import { BrickComponentViewState } from "src/app/components/game/field/brick.component/classes/brick.component.view-state";
import { Difficulty } from "src/app/enums/difficulty";
import { IBrickDecorationService } from "src/app/services/brick-decoration/i-brick-decoration.service";

export class FieldComponentViewState extends ActivatableGameComponentViewState {

    readonly brickComponentViewStateTemplate: BrickComponentViewState;

    readonly fieldContainerBoundaryLength: number;

    readonly fieldContainerSideLength: number;

    readonly fieldSideLength: number;

    constructor(bricksDecorationService: IBrickDecorationService, difficulty: Difficulty, sideBricksCount: number) {
        super();
        this.brickComponentViewStateTemplate = new BrickComponentViewState(bricksDecorationService.getBrickMarginLength(difficulty),
            bricksDecorationService.getBrickSideLength(difficulty));
        this.fieldSideLength = sideBricksCount * (this.brickComponentViewStateTemplate.brickSideLength +
            2 * this.brickComponentViewStateTemplate.brickMarginLength);
        this.fieldContainerSideLength = this.fieldSideLength + 2 * bricksDecorationService.getBoundaryBrickSpaceLength(difficulty);
        this.fieldContainerBoundaryLength = bricksDecorationService.getBoundaryLength(difficulty);
    }
}