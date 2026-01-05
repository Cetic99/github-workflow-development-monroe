/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import LogoSmall from '@ui/components/icons/LogoSmall'
import IconRightHalfArrow from '@ui/components/icons/IconRightHalfArrow'
import { useNavigate } from 'react-router-dom'

const Container = styled.div`
  display: flex;
  align-items: center;
  gap: 0.5rem;

  color: var(--secondary-dark);
  font-weight: 700;
  font-size: 2.5rem;
  line-height: 2.625rem;

  & .logo-wrapper {
    display: flex;
  }

  & .last-breadcrumb {
    max-width: 15.625rem;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
  }
`

const Breadcrumbs = ({ name = '', names = [] }) => {
  const navigate = useNavigate()

  return (
    <Container>
      <div className="logo-wrapper" onClick={() => navigate('/')}>
        <LogoSmall />
      </div>

      <IconRightHalfArrow size="l" />

      {names.length > 0 && <span>{names[0]}</span>}

      {name && (
        <>
          <IconRightHalfArrow size="l" />
          <span className="last-breadcrumb">{name}</span>
        </>
      )}
    </Container>
  )
}

export default Breadcrumbs
