/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import IconInfoSquare from '@ui/components/icons/IconInfoSquare'

const Container = styled.div`
  padding: 0.25rem;
  border-radius: 10px;
  transition: all 0.2s ease;

  &:active {
    background-color: #ededed;
  }
`

const IconButtonCellRender = ({
  className = '',
  size = 'm',
  color = 'black',
  onClick = () => {}
}) => {
  return (
    <Container className={className} onClick={onClick}>
      <IconInfoSquare size={size} color={color} />
    </Container>
  )
}

export default IconButtonCellRender
