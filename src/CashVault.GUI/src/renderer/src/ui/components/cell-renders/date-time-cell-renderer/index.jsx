import styled from '@emotion/styled'

const TwoRowCell = styled.div`
  display: flex;
  flex-direction: column;
  gap: 0.25rem;

  & .date-time-cell {
    font-weight: 500;
    font-size: 1.125rem;
    line-height: 1.5rem;
  }
`

const DateTimeCellRenderer = ({ value }) => {
  const parseTimestamp = (timestamp) => {
    if (timestamp === undefined || timestamp === null)
      return {
        date: '',
        time: ''
      }

    try {
      const dateObj = new Date(timestamp)

      const pad = (n, size = 2) => String(n).padStart(size, '0')

      const date = `${pad(dateObj.getDate())}.${pad(dateObj.getMonth() + 1)}.${dateObj.getFullYear()}`

      const time = `${pad(dateObj.getHours())}:${pad(dateObj.getMinutes())}:${pad(dateObj.getSeconds())}`

      const fractionMatch = timestamp.match(/\.(\d{3})/)
      const milliseconds = fractionMatch ? fractionMatch[1] : '000'

      return {
        date,
        time: `${time}.${milliseconds}`
      }
    } catch (error) {
      console.error('Error parsing timestamp:', error)
      return {
        date: '',
        time: ''
      }
    }
  }

  return (
    <TwoRowCell className="date-time-cell">
      <span>{parseTimestamp(value)?.date}</span>
      <span>{parseTimestamp(value)?.time}</span>
    </TwoRowCell>
  )
}

export default DateTimeCellRenderer
