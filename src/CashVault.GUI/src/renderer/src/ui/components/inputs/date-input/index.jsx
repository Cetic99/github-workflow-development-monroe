/* eslint-disable prettier/prettier */
/* eslint-disable react/prop-types */

import styled from '@emotion/styled'
import DatePicker from 'react-date-picker'
import IconCalendar from '@ui/components/icons/IconCalendar'
import 'react-date-picker/dist/DatePicker.css'
import 'react-calendar/dist/Calendar.css'
import IconLeftHalfArrow from '@ui/components/icons/IconLeftHalfArrow'
import IconRightHalfArrow from '@ui/components/icons/IconRightHalfArrow'
import IconChevronLeft from '@ui/components/icons/IconChevronLeft'
import IconChevronRight from '@ui/components/icons/IconChevronRight'

const Container = styled.div`
  display: flex;
  width: 100%;

  & abbr {
    text-decoration: none;
  }

  & .react-date-picker__calendar {
    width: 100%;
  }

  & .react-calendar {
    border: none;
    border-radius: 10px;
    padding: 1rem;
    position: fixed;
    top: calc(100% / 2 - 250px);
    right: calc(100% / 2 - 250px);
    font-size: 24px;
    width: 500px;
  }

  & .react-date-picker {
    width: 100%;
    background-color: var(--bg-dark);
    padding: 0.7625rem 1rem;
    border-radius: 10px;
  }

  & .react-date-picker__wrapper {
    border: none;
    background-color: var(--bg-dark);
    gap: 0.25rem;
    border-radius: 10px;
  }

  & .react-date-picker__inputGroup {
    display: flex;
    width: 100%;
    align-items: center;
    justify-content: flex-end;

    font-family: Poppins;
    font-weight: 600;
    font-size: 2rem;
    line-height: 2.5rem;
    text-align: right;
  }

  & .react-calendar__navigation__label__labelText {
    font-size: 1.5rem;
  }

  & .react-calendar__tile {
    padding: 1.25rem 0.5rem;
    border-radius: 10px;
  }

  & .react-calendar__tile--active {
    background-color: var(--primary-medium);
  }

  & .react-calendar__tile--hasActive {
    background-color: var(--primary-medium);
    color: white;
  }

  & .react-calendar__tile--now {
    background-color: var(--primary-light);
  }

  & .react-calendar__navigation__arrow {
    border-radius: 10px;
  }

  & .react-calendar__navigation__arrow::active {
    border-radius: 10px;
  }

  & .react-calendar__navigation__label {
    border-radius: 10px;
  }
`

const OuterContainer = styled.div`
  display: flex;
  flex-direction: column;
`

const Label = styled.div`
  font-weight: 600;
  font-size: 1.125rem;
  line-height: 1.375rem;
  color: var(--text-default);
  text-transform: uppercase;
  margin-bottom: 0.5rem;
`

const DateInput = ({
  size = 'l',
  defaultValue,
  value,
  disabled = false,
  onChange = () => {},
  dateFormat = 'DD.MM.YYYY.',
  label,
  className = '',
  rest
}) => {
  const handleValueChange = (value) => {
    const date = new Date(Date.UTC(value.getFullYear(), value.getMonth(), value.getDate()))

    onChange(date)
  }

  return (
    <OuterContainer size={size} className={className}>
      {label && <Label>{label}</Label>}

      <Container size={size}>
        <DatePicker
          onChange={handleValueChange}
          defaultValue={defaultValue}
          value={value}
          calendarIcon={<IconCalendar />}
          closeCalendar={true}
          clearIcon={null}
          format={dateFormat}
          disabled={disabled}
          yearPlaceholder=""
          monthPlaceholder=""
          dayPlaceholder=""
          calendarProps={{
            zIndex: 99999,
            nextLabel: <IconRightHalfArrow size="m" />,
            next2Label: <IconChevronRight size="m" />,
            prevLabel: <IconLeftHalfArrow size="m" />,
            prev2Label: <IconChevronLeft size="m" />
          }}
          readOnly={true}
          {...rest}
        />
      </Container>
    </OuterContainer>
  )
}

export default DateInput
