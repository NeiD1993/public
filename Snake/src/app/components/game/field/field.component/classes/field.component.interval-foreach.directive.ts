import { BaseIntervalStructuralDirective } from 'src/app/directives/base-interval-structural.directive';
import { BrickComponent } from 'src/app/components/game/field/brick.component/classes/brick.component';
import { BrickComponentViewState } from 'src/app/components/game/field/brick.component/classes/brick.component.view-state';
import { Bricks } from 'src/app/models/bricks';
import { ComponentFactoryResolver, ComponentRef, Directive, Input, OnDestroy, TemplateRef, ViewContainerRef } from '@angular/core';
import { FieldComponent } from 'src/app/components/game/field/field.component/classes/field.component';
import { Subscription } from 'rxjs';

@Directive({
    selector: '[fieldIntervalForeach]'
})
export class FieldComponentIntervalForeachDirective extends BaseIntervalStructuralDirective implements OnDestroy {

    private _fieldComponentLoadingContinuedSubscription: Subscription;

    constructor(fieldComponent: FieldComponent, componentFactoryResolver: ComponentFactoryResolver, template: TemplateRef<any>,
        viewContainer: ViewContainerRef) {
        super(componentFactoryResolver, template, viewContainer);
        this._fieldComponentLoadingContinuedSubscription =
            fieldComponent.loadingContinued.subscribe(this.onFieldComponentLoadingContinued.bind(this));
    }

    @Input()
    set fieldIntervalForeach(parameters: {
        condition: boolean,
        fieldParameters: { bricks: Bricks, brickViewState: BrickComponentViewState, fieldComponent: FieldComponent }
    }) {
        if (parameters.condition) {
            this._viewContainer.clear();
            this.startIntervalScheduler(parameters.fieldParameters);
        }
    }

    private insertBrickComponent(indexes: { row: number, column: number }, parameters: { bricks: Bricks, brickViewState: BrickComponentViewState }):
        BrickComponent {
        let brickComponent: ComponentRef<BrickComponent> =
            this._viewContainer.createComponent(this._componentFactoryResolver.resolveComponentFactory(BrickComponent));

        brickComponent.instance.brick = parameters.bricks.elements[indexes.row][indexes.column];
        brickComponent.instance.viewState = new BrickComponentViewState(parameters.brickViewState.brickMarginLength,
            parameters.brickViewState.brickSideLength);
        this._template.createEmbeddedView(brickComponent.location.nativeElement);

        return brickComponent.instance;
    }

    protected callbackFunction(parameters: any): void {
        let bricksParameters = <{
            bricks: Bricks,
            brickViewState: BrickComponentViewState,
            fieldComponent: FieldComponent
        }>parameters;
        let concreteParameters = <{ positionIndex: number, embeddedBrickComponents: Array<BrickComponent> }>this._parameters;
        let columnIndex = concreteParameters.positionIndex;

        if (columnIndex < bricksParameters.bricks.sideBricksCount) {
            for (let rowIndex = 0; rowIndex < bricksParameters.bricks.sideBricksCount; rowIndex++)
                concreteParameters.embeddedBrickComponents.push(this.insertBrickComponent({ row: rowIndex, column: columnIndex },
                    { bricks: bricksParameters.bricks, brickViewState: bricksParameters.brickViewState }));

            this.positionChanged.emit(++concreteParameters.positionIndex);
        }
        else
            this.executionComplete.emit(concreteParameters.embeddedBrickComponents);
    }

    protected formCallbackFunctionParameters(parameters: any): any {
        return parameters;
    }

    protected formParameters(parameters: any): any {
        return { positionIndex: 0, embeddedBrickComponents: new Array<BrickComponent>() };
    }

    protected setInterval(): number {
        return 125;
    }

    ngOnDestroy(): void {
        this._fieldComponentLoadingContinuedSubscription.unsubscribe();
    }

    onFieldComponentLoadingContinued(): void {
        this.runIntervalScheduler(!((<{ positionIndex: number, embeddedBrickComponents: Array<BrickComponent> }>this._parameters).positionIndex < 100));
    }
}