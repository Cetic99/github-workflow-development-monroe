/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'

const Container = styled.div`
  width: 100%;
  background-color: ${(p) => p.color};
  height: 0.7px;
  margin: 0.75rem 0;
`

const Divider = ({ color = 'var(--bg-divider)' }) => {
  return <Container color={color} />
}

export default Divider
