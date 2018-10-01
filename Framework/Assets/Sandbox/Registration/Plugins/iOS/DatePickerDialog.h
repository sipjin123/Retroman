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

#import <UIKit/UIKit.h>

/**
 DatePickerDialog displays an UIDatePicker in a dialog similar in style to UIAlertView/UIAlertController
 */
@interface DatePickerDialog : UIView
typedef void (^DatePickerCallback)(NSDate* __nullable date);

@property (nonatomic,weak) UIDatePicker* __nullable datePicker;

/**
 Initializes a DatePickerDialog
 @param showCancelButton Is the dialog's cancel button visible
 */
- (id _Nonnull)initWithCancelButton:(BOOL)showCancelButton;

/**
 Initializes a DatePickerDialog
 @param locale The locale used by the datepicker on the dialog
 */
- (id _Nonnull)initWithLocale:(nullable NSLocale*)locale;

/**
 Initializes a DatePickerDialog
 @param showCancelButton Is the dialog's cancel button visible
 @param locale The locale used by the datepicker on the dialog
 */
- (id _Nonnull)initWithLocale:(nullable NSLocale*)locale
                 cancelButton:(BOOL)showCancelButton;

/**
 Initializes a DatePickerDialog
 @param textColor The text color used by the dialog
 @param buttonColor The button color used by the dialog
 @param font The font used by the dialog
 @param locale The locale used by the datepicker on the dialog
 @param showCancelButton Is the dialog's cancel button visible
 */
- (id _Nonnull)initWithTextColor:(nullable UIColor*)textColor
                     buttonColor:(nullable UIColor*)buttonColor
                            font:(nullable UIFont*)font
                          locale:(nullable NSLocale*)locale
                    cancelButton:(BOOL)showCancelButton;

/**
 Shows a DatePickerDialog on the current UIWindow
 @param callback The block to execute when the dialog closes. The date parameter will be nil if the cancel button was tapped
 */
- (void)showWithCallback:(nullable DatePickerCallback)callback;

/**
 Shows a DatePickerDialog on the current UIWindow
 @param title     The title to show on the dialog
 @param callback  The block to execute when the dialog closes. The date parameter will be nil if the cancel button was tapped
 */
- (void)showWithTitle:(nonnull NSString*)title
             callback:(nullable DatePickerCallback)callback;

/**
 Shows a DatePickerDialog on the current UIWindow
 @param title                 The title to show on the dialog
 @param doneButtonTitle       The title to show on the done button of the dialog
 @param cancelButtonTitle     The title to show on the cancel button of the dialog
 @param defaultDate           The initially selected date of the dialog
 @param datePickerMode        The type of information displayed on the dialog's picker
 @param callback              The block to execute when the dialog closes. The date parameter will be nil if the cancel button was tapped
 */
- (void)showWithTitle:(nonnull NSString*)title
      doneButtonTitle:(nonnull NSString*)doneButtonTitle
    cancelButtonTitle:(nonnull NSString*)cancelButtonTitle
          defaultDate:(nonnull NSDate* )defaultDate
       datePickerMode:(UIDatePickerMode)datePickerMode
             callback:(nullable DatePickerCallback)callback;

/**
 Shows a DatePickerDialog on the current UIWindow
 @param title                 The title to show on the dialog
 @param doneButtonTitle       The title to show on the done button of the dialog
 @param cancelButtonTitle     The title to show on the cancel button of the dialog
 @param defaultDate           The initially selected date of the dialog
 @param minimumDate           The earliest date selectable on the dialog's picker
 @param maximumDate           The latest date selectable on the dialog's picker
 @param datePickerMode        The type of information displayed on the dialog's picker
 @param callback              The block to execute when the dialog closes. The date parameter will be nil if the cancel button was tapped
 */
- (void)showWithTitle:(nonnull NSString*)title
      doneButtonTitle:(nonnull NSString*)doneButtonTitle
    cancelButtonTitle:(nonnull NSString*)cancelButtonTitle
          defaultDate:(nonnull NSDate*)defaultDate
          minimumDate:(nullable NSDate*)minimumDate
          maximumDate:(nullable NSDate*)maximumDate
       datePickerMode:(UIDatePickerMode)datePickerMode
             callback:(nullable DatePickerCallback)callback;
@end
