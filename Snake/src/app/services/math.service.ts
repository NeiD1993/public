export class MathService {

    static getPseudoRandomNumber(lowerBoundary: number, upperBoundary: number): number {
        return Math.floor(Math.random() * (upperBoundary - lowerBoundary)) + lowerBoundary;
    }

    static powerNumber(number: number, exponent: number): number {
        return Math.pow(number, exponent);
    }

    static roundNumbersDivision(divisibleNumber: number, divisor: number): number {
        return Math.floor(divisibleNumber / divisor);
    }

    getNumberPercentage(number: number, percent: number): number {
        return Math.floor(percent * number);
    }

    roundNumberDivision(divisibleNumber: number, divisor: number): number {
        return MathService.roundNumbersDivision(divisibleNumber, divisor);
    }
}