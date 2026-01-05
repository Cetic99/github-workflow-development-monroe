/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'

const Container = styled.span`
  font-weight: 700;
  font-size: 4.375rem;
  line-height: 5rem;
  letter-spacing: -4%;

  color: ${(p) => (p.isError ? 'var(--error-dark)' : 'black')};
`

const MainText = ({ children, isError }) => {
  return <Container isError={isError}>{children}</Container>
}

export default MainText
