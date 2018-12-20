import { $enum } from "ts-enum-util";
import { MathService } from "./math.service";
import { SettingsOptionType } from "../enums/settings-option-type";

export class SettingsService {

    private static readonly _settingsOptionMetadatas: Map<SettingsOptionType, SettingsOptionMetadata> =
        new Map<SettingsOptionType, SettingsOptionMetadata>([
            [SettingsOptionType.FieldFoodCountPercentage, { minValue: 35, maxValue: 75 }],
            [SettingsOptionType.SnakeInitialLength, { minValue: 3, maxValue: 7 }],
            [SettingsOptionType.SnakeSpeedLevelsCount, { minValue: 4, maxValue: 7 }]
        ]);

    private _settingsOptions: Map<SettingsOptionType, number> = new Map<SettingsOptionType, number>([
        [SettingsOptionType.FieldFoodCountPercentage, SettingsService._settingsOptionMetadatas.get(SettingsOptionType.FieldFoodCountPercentage).minValue],
        [SettingsOptionType.SnakeInitialLength, SettingsService._settingsOptionMetadatas.get(SettingsOptionType.SnakeInitialLength).minValue],
        [SettingsOptionType.SnakeSpeedLevelsCount, SettingsService._settingsOptionMetadatas.get(SettingsOptionType.SnakeSpeedLevelsCount).maxValue]
    ]);

    static isSettingsOptionValueCorrect(settingsOptionType: SettingsOptionType, settingsOptionValue: number): boolean {
        let settingsOptionMetadata: SettingsOptionMetadata = this._settingsOptionMetadatas.get(settingsOptionType);

        return ((settingsOptionValue >= settingsOptionMetadata.minValue) && (settingsOptionValue <= settingsOptionMetadata.maxValue));
    }

    getSettingsOption(settingsOptionType: SettingsOptionType | string): number {
        return this._settingsOptions.get((typeof (settingsOptionType) === 'string') ?
            $enum(SettingsOptionType).getValueOrDefault(settingsOptionType.toString(), SettingsOptionType.SnakeSpeedLevelsCount) :
            <SettingsOptionType>settingsOptionType);
    }

    getSettingsOptionCount(settingsOptionType: SettingsOptionType, settingsOptionChangingStep: number): number {
        if (settingsOptionChangingStep > 0) {
            let settingsOptionMetadata = SettingsService._settingsOptionMetadatas.get(settingsOptionType);

            return (MathService.roundNumbersDivision(settingsOptionMetadata.maxValue - settingsOptionMetadata.minValue,
                settingsOptionChangingStep) + 1);
        }
        else
            return 0;
    }

    getSettingsOptionPosition(settingsOptionType: SettingsOptionType, settingsOptionValue: number, settingsOptionChangingStep: number): number {
        if (SettingsService.isSettingsOptionValueCorrect(settingsOptionType, settingsOptionValue)) {
            let settingsOptionMetadata = SettingsService._settingsOptionMetadatas.get(settingsOptionType);

            return MathService.roundNumbersDivision(settingsOptionValue - settingsOptionMetadata.minValue, settingsOptionChangingStep);
        }
        else
            return -1;
    }

    setSettingsOption(settingsOptionType: SettingsOptionType, value: number): void {
        if (SettingsService.isSettingsOptionValueCorrect(settingsOptionType, value))
            this._settingsOptions.set(settingsOptionType, value);
    }
}

interface SettingsOptionMetadata {

    minValue: number;

    maxValue: number;
}