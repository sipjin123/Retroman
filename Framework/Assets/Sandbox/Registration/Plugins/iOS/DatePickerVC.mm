/*
 * Copyright (C) 2018 Aries Sanchez Sulit
 * Copyright (C) 2018 Synergy88 Digital.
 *
 * This software is provided 'as-is', without any express or implied
 * warranty.  In no event will the authors be held liable for any damages
 * arising from the use of this software.
 *
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 *
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
 */

#import "DatePickerVC.h"

@implementation DatePickerVC

#pragma mark -
#pragma mark Properties
@synthesize datePicker;

- (void) viewDidLoad
{
    [super viewDidLoad];
    
    NSLog(@"Framework::DatePickerVC::viewDidLoad");
    
    UIDatePicker* datePicker = self.datePicker;
    UIView* view = self.view;
    
    datePicker = [[UIDatePicker alloc] init];
    datePicker.datePickerMode = UIDatePickerModeDate;
    //[datePicker addTarget:self action:@selector(datePickerValueChanged:) forControlEvents:UIControlEventValueChanged];
    datePicker.timeZone = [NSTimeZone defaultTimeZone];
    datePicker.minuteInterval = 5;
    
    [view addSubview:datePicker];
}

#pragma mark -
#pragma mark DatePicker Interface Methods

@end
