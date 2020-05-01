import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'enumName'
})

export class EnumNamePipe implements PipeTransform {
    transform(value: number, enumType: any): any {
        return enumType[value].split(/(?=[A-Z])/)
                              .join()
                              .replace(',', ' ');
    }
}
