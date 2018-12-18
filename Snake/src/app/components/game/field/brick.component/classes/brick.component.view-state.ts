import { BaseGameLogicService } from "src/app/services/game-logic/base-game-logic.service";
import { BrickOperationsExecutorService } from "src/app/services/brick-operations-executor.service";
import { BrickType } from "src/app/enums/brick-type";
import { Difficulty } from "src/app/enums/difficulty";
import { Direction } from "src/app/enums/direction";
import { FieldBrick } from "src/app/models/brick/field-brick";
import { GameState } from "src/app/enums/game/game-state";

export class BrickComponentViewState {

    private _brickContainerAnimationViewState: BrickContainerAnimationViewState;

    private _brickMarginLength: number;

    private _brickSideLength: number;

    constructor(brickMarginLength: number, brickSideLength: number) {
        this._brickContainerAnimationViewState = new BrickContainerAnimationViewState();
        this.setBrickMarginLength(brickMarginLength);
        this.setBrickSideLength(brickSideLength);
    }

    private setBrickMarginLength(brickBorderLength: number): void {
        this._brickMarginLength = (brickBorderLength > 0) ? brickBorderLength : 0;
    }

    private setBrickSideLength(brickSideLength: number): void {
        this._brickSideLength = (brickSideLength > 0) ? brickSideLength : 0;
    }

    get brickMarginLength(): number {
        return this._brickMarginLength;
    }

    get brickSideLength(): number {
        return this._brickSideLength;
    }

    getBrickContainerAnimationTriggerParameters(brick: FieldBrick, gameLogicService: BaseGameLogicService): string | {
        value: string,
        params: IBrickContainerAnimationParameters
    } {
        return this._brickContainerAnimationViewState.getAnimationTriggerParameters(brick, gameLogicService);
    }
}

class BrickContainerAnimationViewState {

    private static getAnimationDuration(animationDurationParameters: { initial: number, decrement: number },
        gameLogicService: BaseGameLogicService): number {
        return (animationDurationParameters.initial - (gameLogicService.field.snake.speedLevel - 1) * animationDurationParameters.decrement);
    }

    private static getStringAnimationDuration(gameLogicService: BaseGameLogicService): string {
        let animationDurationParameters: { initial: number, decrement: number };

        switch (gameLogicService.difficulty) {
            case Difficulty.Easy:
                animationDurationParameters = {
                    initial: 500,
                    decrement: 42
                }
                break;
            case Difficulty.Medium:
                animationDurationParameters = {
                    initial: 400,
                    decrement: 37
                }
                break;
            case Difficulty.Hard:
                animationDurationParameters = {
                    initial: 300,
                    decrement: 32
                }
        }

        return this.getAnimationDuration(animationDurationParameters, gameLogicService) + 'ms';
    }

    private static getAnimationParameters(brick: FieldBrick, gameLogicService: BaseGameLogicService): IBrickContainerAnimationParameters {
        let animationProperties: IBrickContainerAnimationParameters = {
            duration: BrickContainerAnimationViewState.getStringAnimationDuration(gameLogicService),
            startHeight: 0,
            endHeight: 0,
            startWidth: 0,
            endWidth: 0,
            horizontalAlign: 'center',
            verticalAlign: 'center'
        };

        if (brick.type == BrickType.Snake) {
            if ((gameLogicService.field.snake.direction == Direction.Down) || (gameLogicService.field.snake.direction == Direction.Up)) {
                animationProperties.startHeight = 0;
                animationProperties.endHeight = '100%';
                animationProperties.startWidth = animationProperties.endHeight;
                animationProperties.endWidth = animationProperties.startWidth;
                animationProperties.horizontalAlign = 'center';

                if (gameLogicService.field.snake.direction == Direction.Down)
                    animationProperties.verticalAlign = 'flex-start';
                else
                    animationProperties.verticalAlign = 'flex-end';
            }
            else {
                animationProperties.startHeight = '100%';
                animationProperties.endHeight = animationProperties.startHeight;
                animationProperties.startWidth = 0;
                animationProperties.endWidth = animationProperties.endHeight;
                animationProperties.verticalAlign = 'center';

                if (gameLogicService.field.snake.direction == Direction.Left)
                    animationProperties.horizontalAlign = 'flex-end';
                else
                    animationProperties.horizontalAlign = 'flex-start';
            }
        }
        else {
            let directionToSnakeTailBrick: Direction =
                BrickOperationsExecutorService.getDirectionBetweenBricks(brick, gameLogicService.field.snake.bricks.tail);

            if ((directionToSnakeTailBrick == Direction.Left) || (directionToSnakeTailBrick == Direction.Right)) {
                animationProperties.startHeight = '100%';
                animationProperties.endHeight = animationProperties.startHeight;
                animationProperties.startWidth = animationProperties.endHeight;
                animationProperties.endWidth = 0;
                animationProperties.verticalAlign = 'center';

                if (directionToSnakeTailBrick == Direction.Right)
                    animationProperties.horizontalAlign = 'flex-start';
                else
                    animationProperties.horizontalAlign = 'flex-end';
            }
            else {
                animationProperties.startHeight = '100%';
                animationProperties.endHeight = 0;
                animationProperties.startWidth = animationProperties.startHeight;
                animationProperties.endWidth = animationProperties.startWidth;
                animationProperties.horizontalAlign = 'center';

                if (directionToSnakeTailBrick == Direction.Up)
                    animationProperties.verticalAlign = 'flex-start';
                else
                    animationProperties.verticalAlign = 'flex-end';
            }
        }

        return animationProperties;
    }

    getAnimationTriggerParameters(brick: FieldBrick, gameLogicService: BaseGameLogicService): string | {
        value: string,
        params: IBrickContainerAnimationParameters
    } {
        if ((gameLogicService.gameState == GameState.NotStarted) || (brick.type == BrickType.Food))
            return brick.typeToString;
        else {
            return {
                value: brick.typeToString,
                params: BrickContainerAnimationViewState.getAnimationParameters(brick, gameLogicService)
            };
        }
    }
}

export interface IBrickContainerAnimationParameters {

    duration: string,

    startHeight: string | number,

    endHeight: string | number,

    startWidth: string | number,

    endWidth: string | number,

    horizontalAlign: string,

    verticalAlign: string
}